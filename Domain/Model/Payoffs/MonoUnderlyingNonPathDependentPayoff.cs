namespace Domain {
    public class MonoUnderlyingNonPathDependentPayoff : INonPathDependentPayoff {
        private readonly Func<double, double> _payoffMap;
        private readonly Underlying _underlying;
        public MonoUnderlyingNonPathDependentPayoff(Func<double, double> payoffMap, Underlying underlying) {
            _payoffMap = payoffMap;
            _underlying = underlying;
        }
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return _payoffMap(pricesAtMaturity[_underlying]);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return new List<Underlying>() { _underlying };
        }
    }
}
