using Application;
using Domain;
using PricerServices.Pricers;

namespace PricerServices {
    public class PricingEngine {

        private ModelConfiguration _config;
        private IMarketData _marketData;
        public PricingEngine(ModelConfiguration configuration) {
            _config = configuration;
        }

        public PricingEngine SetMarketData(IMarketData marketData) {
            _marketData = marketData;
            return this;
        }

        public static void Run(PricingRequest request) {
            PricingEngine engine = new PricingEngine(request.ModelConfiguration)
                .SetMarketData(request.MarketData);
            HashSet<IMarketData> shiftedMarketData = new HashSet<IMarketData>();
            shiftedMarketData.Add(request.MarketData);
            foreach (IIndicator indicator in request.Indicators) {
                indicator.GetShiftedMarketData().ForEach(md => shiftedMarketData.Add(md));
            }
            Dictionary<IMarketData, Dictionary<Contract, ValueWithPrecision>> subResults = new();
            foreach(IMarketData marketData in shiftedMarketData) {
                foreach(Contract contract in request.Position) {
                    if (contract is NonPathDependentContract nonPathDependentContract) {
                        ValueWithPrecision result = engine.Price(nonPathDependentContract.Payoff, nonPathDependentContract.Maturity, request.PricingDate);
                        subResults.Add(marketData, new Dictionary<Contract, ValueWithPrecision>)
                    }
                }
            }
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
