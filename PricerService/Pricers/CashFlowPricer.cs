using Application;
using Domain;
using MarketDataServices;

namespace PricerServices {
    public class CashFlowPricer : IPricer<CashFlow> {
        public CashFlowPricer(MarketDataService marketDataService) {
        }

        public double Price(CashFlow contract) {
            // todo : implement discounted cash flow
            return contract.Values.Sum();
        }
    }
}
