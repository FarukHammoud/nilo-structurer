using Domain;

namespace Application {
    public interface IMultiUnderlyingPricer<T1, T2> where T1 : INonPathDependentPayoff where T2 : IMarketData {
        ValueWithPrecision Price(T1 payoff, T2 MarketData, DateTime maturity, DateTime today);
    }
}