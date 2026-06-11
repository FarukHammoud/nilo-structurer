using Domain;

namespace Application {
    public class PricingEngine : IPricingEngine {

        private readonly IPricerFactory _pricerFactory;
        private readonly ITimeGridBuilder _timeGridBuilder;

        public PricingEngine(IPricerFactory? pricerFactory = null, ITimeGridBuilder? timeGridBuilder = null) {
            _pricerFactory = pricerFactory ?? new PricerFactory();
            _timeGridBuilder = timeGridBuilder ?? new TimeGridBuilder();
        }

        // Target signature for the asynchronous pricing method
        public async Task<Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>>> RunAsync(PricingRequest request,
            IProgress<PricingProgress>? progress = null,
            CancellationToken cancellationToken = default) {
            return await Task.Run(() => {
                return Run(request);
            }, cancellationToken);
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
                IEnumerable<DateTime> maturities = pathIndependentContracts.SelectMany(contract => contract.Dates).Distinct();
                PriceContracts(pathIndependentContracts, pricer, pricerConfiguration, request.PricingDate, shiftedMarketData, subResults, request.PricingCurrency);
            }
            if (pathDependentContracts.Any()) {
                IPayoffPricer<IPathDependentPayoff> pricer = _pricerFactory.CreatePathDependentPricer(request.ModelConfiguration);
                IEnumerable<DateTime> maturities = pathDependentContracts.SelectMany(contract => contract.Dates).Distinct();
                PriceContracts(pathDependentContracts, pricer, pricerConfiguration, request.PricingDate, shiftedMarketData, subResults, request.PricingCurrency);
            }

            return GetIndicatorResults(request, subResults);
        }

        private void PriceContracts<T>(
            IEnumerable<IGeneralContract<T>> contracts,
            IPayoffPricer<T> pricer,
            IPricerConfiguration? config,
            DateTime valuationDate,
            HashSet<(IMarketData, DateTime)> shiftedMarketData,
            Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults,
            Currency pricingCurrency) where T : IPayoff {
            IEnumerable<DateTime> timeGrid = _timeGridBuilder.Build(contracts, valuationDate);
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
            IDiscounter discounter = marketData.GetDiscounter(pricingCurrency);
            double price = 0.0, precisionSquared = 0.0;
            foreach (T payoff in contract.Payoffs) {
                PriceWithPrecision payoffPv = pricer.Price(payoff, discounter, marketData, payoff.PaymentDate, pricingDate, pricingCurrency);
                double fxRate = marketData.GetFxRate(payoffPv.Currency, pricingCurrency);
                price += payoffPv.Value * fxRate;
                precisionSquared += Math.Pow(payoffPv.Precision * fxRate, 2);
            }
            return new PriceWithPrecision() {
                Value = price,
                Precision = Math.Sqrt(precisionSquared),
                Currency = pricingCurrency
            };
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
