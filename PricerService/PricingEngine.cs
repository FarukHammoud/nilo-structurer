using Application;
using Domain;
using PricerServices.Pricers;

namespace PricerServices {
    public class PricingEngine {

        private ModelConfiguration _config;
        private IMarketData? _marketData;
        public PricingEngine(ModelConfiguration configuration) {
            _config = configuration;
        }

        private PricingEngine SetMarketData(IMarketData marketData) {
            _marketData = marketData;
            return this;
        }

        public static Dictionary<IContract, Dictionary<IIndicator, ValueWithPrecision>> Run(PricingRequest request) {
            PricingEngine engine = new PricingEngine(request.ModelConfiguration);
            HashSet<IMarketData> shiftedMarketData = request.Indicators
                .SelectMany(indicator => indicator.GetShiftedMarketData(request.MarketData))
                .ToHashSet();
            Dictionary<IMarketData, Dictionary<IContract, ValueWithPrecision>> subResults = new();
            foreach(IMarketData marketData in shiftedMarketData) {
                engine.SetMarketData(marketData);
                Dictionary<IContract, ValueWithPrecision> resultByContract = new();
                foreach (IContract contract in request.Position) {
                    if (contract is INonPathDependentContract nonPathDependentContract) {
                        // ALERT: Diffusion is done several times
                        // Initialize / Price Differentiation
                        IEnumerable<ValueWithPrecision> payoffsValues = nonPathDependentContract.Payoffs.Select(
                            payoff => engine.Price(payoff.Item2, payoff.Item1, request.PricingDate));
                        ValueWithPrecision aggregatedPayoffValue = new ValueWithPrecision {
                            Value = payoffsValues.Sum(pv => pv.Value),
                            Precision = Math.Sqrt(payoffsValues.Sum(pv => Math.Pow(pv.Precision, 2)))
                        };
                        resultByContract.Add(contract, aggregatedPayoffValue);
                    }
                }
                subResults.Add(marketData, resultByContract);
            }

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

        public ValueWithPrecision Price(INonPathDependentPayoff payoff, DateTime maturity, DateTime today) {
            if (_config.Pricing is MonteCarlo) {
                IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> pricer = new GeneralDiffusionPricer();
                return pricer.Price(payoff, _marketData, maturity, today);
            }
            return new ValueWithPrecision { Value = 0, Precision = 0 };
        }
    }
}
