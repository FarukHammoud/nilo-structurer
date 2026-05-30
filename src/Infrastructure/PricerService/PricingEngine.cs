using Application;
using Domain;

namespace PricerServices {
    public class PricingEngine : IPricingEngine {

        private readonly IPricerFactory _pricerFactory;

        public PricingEngine(IPricerFactory? pricerFactory = null) {
            _pricerFactory = pricerFactory ?? new PricerFactory();
        }

        private Func<IEnumerable<DateTime>, DateTime, List<DateTime>> TimeDiscretizationFactory = (maturities, pricingDate) => Enumerable.Range(0, (int)(maturities.Max() - pricingDate).TotalDays + 1)
                        .Select(i => pricingDate.AddDays(i))
                        .ToList();

        private Func<IEnumerable<DateTime>, DateTime, List<DateTime>> PathIndependentTimeDiscretizationFactory = (maturities, pricingDate) => new List<DateTime>() { pricingDate }.Union(maturities)
                        .ToList();

        // Target signature for the asynchronous pricing method
        public async Task<PricingResult> RunAsync(PricingRequest request,
            IProgress<PricingProgress>? progress = null,
            CancellationToken cancellationToken = default) {
            return new PricingResult {
                Price = new ValueWithPrecision {
                    Value = 0, // Placeholder for the actual price
                    Precision = 0 // Placeholder for the actual precision
                },
                ComputeTime = TimeSpan.Zero // Placeholder for the actual compute time
            };
        }

        public Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> Run(PricingRequest request) {
            Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults = new();

            HashSet<(IMarketData, DateTime)> shiftedMarketData = request.Indicators
                .SelectMany(indicator => indicator.GetShiftedMarketData(request.MarketData, request.PricingDate))
                .ToHashSet();

            IEnumerable<IPathIndependentContract> pathIndependentContracts = request.Position.OfType<IPathIndependentContract>();
            IEnumerable<IPathDependentContract> pathDependentContracts = request.Position.OfType<IPathDependentContract>();

            IPricerConfiguration pricerConfiguration = _pricerFactory.CreateConfiguration(request);
            if (pathIndependentContracts.Any()) {
                IPayoffPricer<IPathIndependentPayoff> pricer = _pricerFactory.CreatePathIndependentPricer(request.ModelConfiguration);
                IEnumerable<DateTime> maturities = pathIndependentContracts.SelectMany(contract => contract.Payoffs.Select(payoff => payoff.Maturity)).Distinct();
                PriceContracts(pathIndependentContracts, pricer, pricerConfiguration, PathIndependentTimeDiscretizationFactory(maturities, request.PricingDate), shiftedMarketData, subResults, request.PricingCurrency);
            }
            if (pathDependentContracts.Any()) {
                IPayoffPricer<IPathDependentPayoff> pricer = _pricerFactory.CreatePathDependentPricer(request.ModelConfiguration);
                IEnumerable<DateTime> maturities = pathDependentContracts.SelectMany(contract => contract.Payoffs.Select(payoff => payoff.PaymentDate)).Distinct();
                PriceContracts(pathDependentContracts, pricer, pricerConfiguration, TimeDiscretizationFactory(maturities, request.PricingDate), shiftedMarketData, subResults, request.PricingCurrency);
            }

            return GetIndicatorResults(request, subResults);
        }

        private void PriceContracts<T>(
            IEnumerable<IGeneralContract<T>> contracts,
            IPayoffPricer<T> pricer,
            IPricerConfiguration? config,
            IEnumerable<DateTime> timeGrid,
            HashSet<(IMarketData, DateTime)> shiftedMarketData,
            Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults,
            Currency pricingCurrency) where T : IPayoff {
            if (!contracts.Any()) return;
            foreach ((IMarketData marketData, DateTime pricingDate) in shiftedMarketData) {
                pricer.Initialize(marketData, timeGrid.ToList(), config);
                subResults[(marketData, pricingDate)] = contracts.ToDictionary(
                    c => (IContract)c,
                    c => PriceContract(pricer, c, marketData, pricingDate, pricingCurrency));
            }
        }

        private PriceWithPrecision PriceContract<T>(
            IPayoffPricer<T> pricer,
            IGeneralContract<T> contract,
            IMarketData marketData,
            DateTime pricingDate,
            Currency pricingCurrency) where T : IPayoff {
            IEnumerable<PriceWithPrecision> payoffPrices = contract.Payoffs.Select(
                                            payoff => pricer.Price(payoff, marketData.GetDiscounter(pricingCurrency), marketData, payoff.PaymentDate, pricingDate, pricingCurrency)).ToList();
            PriceWithPrecision aggregatedPayoffValue = new() {
                Value = payoffPrices.Sum(price => price.Value * marketData.GetFxRate(price.Currency, pricingCurrency)),
                Precision = Math.Sqrt(payoffPrices.Sum(pv => Math.Pow(pv.Precision, 2))),
                Currency = pricingCurrency
            };
            return aggregatedPayoffValue;
        }

        private Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> GetIndicatorResults(PricingRequest request, Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults) {
            // Transform subResults to the desired output format
            Dictionary<IContract, Dictionary<(IMarketData, DateTime), PriceWithPrecision>> pivotedSubResults = subResults.Pivot();
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> indicatorResult = new();
            foreach (IContract contract in request.Position) {
                indicatorResult.Add(contract, new());
                foreach (IIndicator indicator in request.Indicators) {
                    IIndicatorResult result = indicator.GetResult(
                        contract: contract,
                        unshiftedMarketData: request.MarketData,
                        pricingDate: request.PricingDate,
                        resultsByShift: pivotedSubResults[contract]);
                    indicatorResult[contract].Add(indicator, result);
                }
            }
            return indicatorResult;
        }
    }
}
