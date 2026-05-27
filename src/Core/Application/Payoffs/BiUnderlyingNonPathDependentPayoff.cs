using Domain;

namespace Application {
    public class BiUnderlyingNonPathDependentPayoff : INonPathDependentPayoff {
        private readonly Func<double, double, double> _payoffMap;
        private readonly Underlying _firstUnderlying;
        private readonly Underlying _secondUnderlying;
        private readonly Currency _currency;
        public BiUnderlyingNonPathDependentPayoff(Func<double, double, double> payoffMap, Underlying firstUnderlying, Underlying secondUnderlying, Currency currency) {
            _payoffMap = payoffMap;
            _firstUnderlying = firstUnderlying;
            _secondUnderlying = secondUnderlying;
            _currency = currency;
        }
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            double firstUnderlyingValue = ((INonPathDependentPayoff)this).GetUnderlyingValue(_firstUnderlying, pricesAtMaturity);
            double secondUnderlyingValue = ((INonPathDependentPayoff)this).GetUnderlyingValue(_secondUnderlying, pricesAtMaturity);
            return _payoffMap(firstUnderlyingValue, secondUnderlyingValue);
        }

        public IEnumerable<Underlying> Dependencies =>
                _firstUnderlying.Dependencies
            .Union(_secondUnderlying.Dependencies);

        public Currency Currency => _currency;
    }
}
