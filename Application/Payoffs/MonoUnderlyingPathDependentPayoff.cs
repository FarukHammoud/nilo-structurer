using Domain;

namespace Application {
    public class MonoUnderlyingPathDependentPayoff : IPathDependentPayoff {
        private readonly Func<Dictionary<DateTime, double>, double> _payoffMap;
        private readonly Underlying _underlying;
        private readonly List<DateTime> _datesOfInterest;
        public MonoUnderlyingPathDependentPayoff(Func<Dictionary<DateTime, double>, double> payoffMap, List<DateTime> datesOfInterest, Underlying underlying) {
            _payoffMap = payoffMap;
            _datesOfInterest = datesOfInterest;
            _underlying = underlying;
        }

        public List<DateTime> GetDatesOfInterest() {
            return _datesOfInterest;
        }

        public double GetPayoffAtMaturity(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> underlyingValues = ((IPathDependentPayoff)this).GetUnderlyingValues(_underlying, prices);
            return _payoffMap(underlyingValues);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return _underlying.GetUnderlyingDependencyList();
        }
    }
}
