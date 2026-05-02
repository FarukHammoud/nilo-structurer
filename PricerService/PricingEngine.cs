using Application;
using Domain;
using PricerServices.Pricers;

namespace PricerServices {
    public class PricingEngine : IPricingEngine {

        private Func<IEnumerable<DateTime>, DateTime, List<DateTime>> TimeDiscretizationFactory = (maturities, pricingDate) => Enumerable.Range(0, (int)(maturities.Max() - pricingDate).TotalDays + 1)
                        .Select(i => pricingDate.AddDays(i))
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
            PricingEngine engine = new PricingEngine();
            HashSet<(IMarketData, DateTime)> shiftedMarketData = request.Indicators
                .SelectMany(indicator => indicator.GetShiftedMarketData(request.MarketData, request.PricingDate))
                .ToHashSet();
            
            IEnumerable<INonPathDependentContract> nonPathDependentContracts = request.Position.OfType<INonPathDependentContract>();
            IEnumerable<IPathDependentContract> pathDependentContracts = request.Position.OfType<IPathDependentContract>();

            IEnumerable<DateTime> maturities = nonPathDependentContracts.SelectMany(contract => contract.Payoffs.Select(payoff => payoff.Item1)).Distinct();
            IEnumerable<DateTime> pathDependentMaturities = pathDependentContracts.SelectMany(contract => contract.Payoffs.Select(payoff => payoff.Item1)).Distinct();
            Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults = new();
            if (request.ModelConfiguration.Pricing is MonteCarlo) {
                if (nonPathDependentContracts.Any()) {
                    INonPathDependentPricer pricer = new NonPathDependentDiffusionPricer();
                    foreach ((IMarketData marketData, DateTime pricingDate) in shiftedMarketData) {
                        pricer.Initialize(marketData, TimeDiscretizationFactory(maturities, request.PricingDate));
                        Dictionary<IContract, PriceWithPrecision> resultByContract = nonPathDependentContracts.ToDictionary(contract => (IContract)contract, contract => PriceContract(pricer, contract, marketData, pricingDate, request.PricingCurrency));
                        subResults.Add((marketData, pricingDate), resultByContract);
                    }
                }
                if (pathDependentContracts.Any()) {
                    IPathDependentPricer pathDependentPricer = new PathDependentDiffusionPricer();
                    foreach ((IMarketData marketData, DateTime pricingDate) in shiftedMarketData) {
                        pathDependentPricer.Initialize(marketData, TimeDiscretizationFactory(pathDependentMaturities, request.PricingDate));
                        Dictionary<IContract, PriceWithPrecision> resultByContract = pathDependentContracts.ToDictionary(contract => (IContract)contract, contract => PricePathDependentContract(pathDependentPricer, contract, marketData, pricingDate, request.PricingCurrency));
                        subResults.Add((marketData, pricingDate), resultByContract);
                    }
                }
            }
            if (request.ModelConfiguration.Pricing is BinaryTree) {
                INonPathDependentPricer pricer = new BinaryTreePricer();
                foreach ((IMarketData marketData, DateTime pricingDate) in shiftedMarketData) {
                    pricer.Initialize(marketData, TimeDiscretizationFactory(maturities, request.PricingDate));
                    Dictionary<IContract, PriceWithPrecision> resultByContract = nonPathDependentContracts.ToDictionary(contract => (IContract)contract, contract => PriceContract(pricer, contract, marketData, pricingDate, request.PricingCurrency));
                    subResults.Add((marketData, pricingDate), resultByContract);
                }
            }

            return GetIndicatorResults(request, subResults);
        }

        private PriceWithPrecision PriceContract(
            INonPathDependentPricer pricer, 
            INonPathDependentContract nonPathDependentContract, 
            IMarketData marketData,
            DateTime pricingDate,
            Currency pricingCurrency) {
            IEnumerable<PriceWithPrecision> payoffPrices = nonPathDependentContract.Payoffs.Select(
                                            payoff => pricer.Price(payoff.Item2, marketData.GetDiscounter(payoff.Item2.Currency), payoff.Item1, pricingDate)).ToList();
            PriceWithPrecision aggregatedPayoffValue = new() {
                Value = payoffPrices.Sum(price => price.Value * marketData.GetFxRate(price.Currency, pricingCurrency)),
                Precision = Math.Sqrt(payoffPrices.Sum(pv => Math.Pow(pv.Precision, 2))),
                Currency = pricingCurrency
            };
            return aggregatedPayoffValue;
        }

        private PriceWithPrecision PricePathDependentContract(
            IPathDependentPricer pricer, 
            IPathDependentContract pathDependentContract, 
            IMarketData marketData,
            DateTime pricingDate,
            Currency pricingCurrency) {
            IEnumerable<PriceWithPrecision> payoffPrices = pathDependentContract.Payoffs.Select(
                                            payoff => pricer.Price(payoff.Item2, marketData.GetDiscounter(payoff.Item2.Currency), marketData, payoff.Item1, pricingDate, pricingCurrency)).ToList();
            PriceWithPrecision price = new() {
                Value = payoffPrices.Sum(price => price.Value * marketData.GetFxRate(price.Currency, pricingCurrency)),
                Precision = Math.Sqrt(payoffPrices.Sum(pv => Math.Pow(pv.Precision, 2))),
                Currency = pricingCurrency
            };
            return price;
        }

        private Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> GetIndicatorResults(PricingRequest request, Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults) {
            // Transform subResults to the desired output format
            Dictionary<IContract, Dictionary<(IMarketData, DateTime), PriceWithPrecision>> pivotedSubResults = subResults.Pivot();
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> indicatorResult = new();
            foreach (IContract contract in request.Position) {
                indicatorResult.Add(contract, new());
                foreach (IIndicator indicator in request.Indicators) {
                    indicatorResult[contract].Add(indicator, indicator.GetResult(request.MarketData, request.PricingDate, pivotedSubResults[contract]));
                }
            }
            return indicatorResult;
        }
    }
}
