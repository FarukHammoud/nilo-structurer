using Domain;

namespace Application {
    public interface IPathDependentPricer {
        void Initialize(IMarketData marketData, List<DateTime> timeDiscretization);
        ValueWithPrecision Price(IPathDependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today);
    }
}