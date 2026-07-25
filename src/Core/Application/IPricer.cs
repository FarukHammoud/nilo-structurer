using Domain;

namespace Application {
    public interface IPricer {
        void Initialize(IMarketData marketData, IList<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null);
        PriceEstimate Price(IContract contract, DateTime today, Currency pricingCurrency);
    }
}