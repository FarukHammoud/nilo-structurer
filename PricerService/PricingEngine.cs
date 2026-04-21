using Application;
using Domain;
using PricerServices.Pricers;

namespace PricerServices {
    public class PricingEngine : IPricingEngine {

        private static Func<IEnumerable<DateTime>, DateTime, List<DateTime>> TimeDiscretizationFactory = (maturities, pricingDate) => Enumerable.Range(0, (int)(maturities.Max() - pricingDate).TotalDays + 1)
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

        public static Dictionary<IContract, Dictionary<IIndicator, ValueWithPrecision>> Run(PricingRequest request) {
            PricingEngine engine = new PricingEngine();
            HashSet<IMarketData> shiftedMarketData = request.Indicators
                .SelectMany(indicator => indicator.GetShiftedMarketData(request.MarketData))
                .ToHashSet();
            
            IEnumerable<INonPathDependentContract> nonPathDependentContracts = request.Position.OfType<INonPathDependentContract>();
            IEnumerable<IPathDependentContract> pathDependentContracts = request.Position.OfType<IPathDependentContract>();

            IEnumerable<DateTime> maturities = nonPathDependentContracts.SelectMany(contract => contract.Payoffs.Select(payoff => payoff.Item1)).Distinct();
            IEnumerable<DateTime> pathDependentMaturities = pathDependentContracts.SelectMany(contract => contract.Payoffs.Select(payoff => payoff.Item1)).Distinct();
            Dictionary<IMarketData, Dictionary<IContract, ValueWithPrecision>> subResults = new();
            if (request.ModelConfiguration.Pricing is MonteCarlo) {
                if (nonPathDependentContracts.Any()) {
                    INonPathDependentPricer pricer = new NonPathDependentDiffusionPricer();
                    foreach (IMarketData marketData in shiftedMarketData) {
                        pricer.Initialize(marketData, TimeDiscretizationFactory(maturities, request.PricingDate));
                        Dictionary<IContract, ValueWithPrecision> resultByContract = nonPathDependentContracts.ToDictionary(contract => (IContract)contract, contract => PriceContract(request, pricer, contract));
                        subResults.Add(marketData, resultByContract);
                    }
                }
                if (pathDependentContracts.Any()) {
                    IPathDependentPricer pathDependentPricer = new PathDependentDiffusionPricer();
                    foreach (IMarketData marketData in shiftedMarketData) {
                        pathDependentPricer.Initialize(marketData, TimeDiscretizationFactory(pathDependentMaturities, request.PricingDate));
                        Dictionary<IContract, ValueWithPrecision> resultByContract = pathDependentContracts.ToDictionary(contract => (IContract)contract, contract => PricePathDependentContract(request, pathDependentPricer, contract));
                        subResults.Add(marketData, resultByContract);
                    }
                }
            }
            if (request.ModelConfiguration.Pricing is BinaryTree) {
                INonPathDependentPricer pricer = new BinaryTreePricer();
                foreach (IMarketData marketData in shiftedMarketData) {
                    pricer.Initialize(marketData, TimeDiscretizationFactory(maturities, request.PricingDate));
                    Dictionary<IContract, ValueWithPrecision> resultByContract = nonPathDependentContracts.ToDictionary(contract => (IContract)contract, contract => PriceContract(request, pricer, contract));
                    subResults.Add(marketData, resultByContract);
                }
            }

            return GetIndicatorResults(request, subResults);
        }

        private static ValueWithPrecision PriceContract(PricingRequest request, INonPathDependentPricer pricer, INonPathDependentContract nonPathDependentContract) {
            IEnumerable<ValueWithPrecision> payoffsValues = nonPathDependentContract.Payoffs.Select(
                                            payoff => pricer.Price(payoff.Item2, request.MarketData, payoff.Item1, request.PricingDate)).ToList();
            ValueWithPrecision aggregatedPayoffValue = new ValueWithPrecision {
                Value = payoffsValues.Sum(pv => pv.Value),
                Precision = Math.Sqrt(payoffsValues.Sum(pv => Math.Pow(pv.Precision, 2)))
            };
            return aggregatedPayoffValue;
        }

        private static ValueWithPrecision PricePathDependentContract(PricingRequest request, IPathDependentPricer pricer, IPathDependentContract pathDependentContract) {
            IEnumerable<ValueWithPrecision> payoffsValues = pathDependentContract.Payoffs.Select(
                                            payoff => pricer.Price(payoff.Item2, request.MarketData, payoff.Item1, request.PricingDate)).ToList();
            ValueWithPrecision aggregatedPayoffValue = new ValueWithPrecision {
                Value = payoffsValues.Sum(pv => pv.Value),
                Precision = Math.Sqrt(payoffsValues.Sum(pv => Math.Pow(pv.Precision, 2)))
            };
            return aggregatedPayoffValue;
        }

        private static Dictionary<IContract, Dictionary<IIndicator, ValueWithPrecision>> GetIndicatorResults(PricingRequest request, Dictionary<IMarketData, Dictionary<IContract, ValueWithPrecision>> subResults) {
            // Transform subResults to the desired output format
            Dictionary<IContract, Dictionary<IMarketData, ValueWithPrecision>> pivotedSubResults = subResults.Pivot();
            Dictionary<IContract, Dictionary<IIndicator, ValueWithPrecision>> indicatorResult = new();
            foreach (IContract contract in request.Position) {
                indicatorResult.Add(contract, new());
                foreach (IIndicator indicator in request.Indicators) {
                    indicatorResult[contract].Add(indicator, indicator.GetResult(request.MarketData, pivotedSubResults[contract]));
                }
            }
            return indicatorResult;
        }
    }
}
