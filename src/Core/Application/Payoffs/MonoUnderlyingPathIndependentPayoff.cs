using Domain;

namespace Application {
    public class MonoUnderlyingPathIndependentPayoff : IPathIndependentPayoff {
        private readonly Func<double, double> _payoffMap;
        private readonly Underlying _underlying;
        private readonly Currency _currency;
        public MonoUnderlyingPathIndependentPayoff(Func<double, double> payoffMap, Underlying underlying, Currency currency) {
            _payoffMap = payoffMap;
            _underlying = underlying;
            _currency = currency;
        }
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {    
            double underlyingValue = _underlying.GetValue(pricesAtMaturity);
            return _payoffMap(underlyingValue);
        }

        public IEnumerable<Underlying> Dependencies => _underlying.Dependencies;
        public Currency Currency => _currency;
    }
}
