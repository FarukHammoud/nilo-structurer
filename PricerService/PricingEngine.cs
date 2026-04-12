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

        public static Dictionary<Contract, Dictionary<IIndicator, ValueWithPrecision>> Run(PricingRequest request) {
            PricingEngine engine = new PricingEngine(request.ModelConfiguration);
            HashSet<IMarketData> shiftedMarketData = request.Indicators
                .SelectMany(indicator => indicator.GetShiftedMarketData(request.MarketData))
                .ToHashSet();
            Dictionary<IMarketData, Dictionary<Contract, ValueWithPrecision>> subResults = new();
            foreach(IMarketData marketData in shiftedMarketData) {
                engine.SetMarketData(marketData);
                Dictionary<Contract, ValueWithPrecision> resultByContract = new();
                foreach (Contract contract in request.Position) {
                    if (contract is NonPathDependentContract nonPathDependentContract) {
                        // ALERT: Diffusion is done several times
                        // Initialize / Price Differentiation
                        ValueWithPrecision result = engine.Price(nonPathDependentContract.Payoff, nonPathDependentContract.Maturity, request.PricingDate);
                        resultByContract.Add(contract, result);
                    }
                }
                subResults.Add(marketData, resultByContract);
            }

            // Transform subResults to the desired output format
            Dictionary<Contract, Dictionary<IMarketData, ValueWithPrecision>> pivotedSubResults = subResults.Pivot();
            Dictionary<Contract, Dictionary<IIndicator, ValueWithPrecision>> indicatorResult = new();
            foreach (Contract contract in request.Position) {
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
