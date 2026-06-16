using Domain;

namespace Application {
    public interface IPricer {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null);
        PriceWithPrecision PricePayoff(IPayoff payoff, DateTime today, Currency pricingCurrency);
    }

    public interface IPricer<TPayoff> : IPricer where TPayoff : IPayoff {
        PriceWithPrecision PricePayoff(TPayoff payoff, DateTime today, Currency pricingCurrency);
    }
}