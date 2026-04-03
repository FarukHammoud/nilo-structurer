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
            return _payoffMap(pricesAtMaturity[_firstUnderlying], pricesAtMaturity[_secondUnderlying]);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return new List<Underlying>() { _firstUnderlying, _secondUnderlying };
        }
    }
}
