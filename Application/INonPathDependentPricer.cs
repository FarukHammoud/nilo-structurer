using Domain;

namespace Application {
    public interface INonPathDependentPricer {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization);
        PriceWithPrecision Price(INonPathDependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today);
    }
}