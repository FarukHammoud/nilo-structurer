using Domain;

namespace Application {
    public interface IPayoffPricer<T> where T : IPayoff {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null);
        PriceWithPrecision Price(T payoff, IDiscounter discounter, IFxConverter fxConverter, DateTime maturity, DateTime today, Currency pricingCurrency);
    }
}