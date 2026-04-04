namespace Domain {
    public class MonoUnderlyingNonPathDependentPayoff : INonPathDependentPayoff {
        private readonly Func<double, double> _payoffMap;
        private readonly Underlying _underlying;
        public MonoUnderlyingNonPathDependentPayoff(Func<double, double> payoffMap, Underlying underlying) {
            _payoffMap = payoffMap;
            _underlying = underlying;
        }
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {    
            double underlyingValue = ((INonPathDependentPayoff)this).GetUnderlyingValue(_underlying, pricesAtMaturity);
            return _payoffMap(underlyingValue);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return _underlying.GetUnderlyingDependencyList();
        }
    }
}
