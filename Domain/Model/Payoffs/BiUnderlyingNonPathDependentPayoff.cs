namespace Domain {
    public class BiUnderlyingNonPathDependentPayoff : INonPathDependentPayoff {
        private readonly Func<double, double, double> _payoffMap;
        private readonly Underlying _firstUnderlying;
        private readonly Underlying _secondUnderlying;
        public BiUnderlyingNonPathDependentPayoff(Func<double, double, double> payoffMap, Underlying firstUnderlying, Underlying secondUnderlying) {
            _payoffMap = payoffMap;
            _firstUnderlying = firstUnderlying;
            _secondUnderlying = secondUnderlying;
        }
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            double firstUnderlyingValue = ((INonPathDependentPayoff)this).GetUnderlyingValue(_firstUnderlying, pricesAtMaturity);
            double secondUnderlyingValue = ((INonPathDependentPayoff)this).GetUnderlyingValue(_secondUnderlying, pricesAtMaturity);
            return _payoffMap(firstUnderlyingValue, secondUnderlyingValue);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return _firstUnderlying.GetUnderlyingDependencyList().Union(_secondUnderlying.GetUnderlyingDependencyList()).ToList();
        }
    }
}
