using Domain;

namespace Application {
    public interface IPathDependentPricer {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization);
        PriceWithPrecision Price(IPathDependentPayoff payoff, IDiscounter discounter, IFxConverter fxConverter,  DateTime maturity, DateTime today, Currency pricingCurrency);
    }
}