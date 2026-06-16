using Domain;

namespace Application {
    public interface IPricer<T> where T : IPayoff {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null);
        PriceWithPrecision PricePayoff(T payoff, DateTime today, Currency pricingCurrency);
    }
}