using Domain;

namespace Application {
    // Target IPricer
    public interface IFlowsPricer {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null);
        PriceWithPrecision Price(IContract contract, DateTime today, Currency pricingCurrency);
    }
}