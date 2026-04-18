using Domain;

namespace Application {
    public interface INonPathDependentPricer {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization);
        ValueWithPrecision Price(INonPathDependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today);
    }
}