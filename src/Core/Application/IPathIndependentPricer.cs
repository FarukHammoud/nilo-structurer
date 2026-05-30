using Domain;

namespace Application {
    public interface IPathIndependentPricer {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null);
        PriceWithPrecision Price(IPathIndependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today);
    }
}