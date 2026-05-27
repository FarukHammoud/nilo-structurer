using Application;
using Domain;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace PricerServices {
    public class FiniteDifferencePdeSolver : INonPathDependentPricer {
        private IMarketData _marketData;
        private List<DateTime> _timeDiscretization;

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            _marketData = marketData;
            _timeDiscretization = timeDiscretization;
        }

        public PriceWithPrecision Price(INonPathDependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today) {
            throw new NotImplementedException();
        }
    }
}
