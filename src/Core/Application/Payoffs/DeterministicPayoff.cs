using Domain;

namespace Application {
    public class DeterministicPayoff : INonPathDependentPayoff {
        private readonly double _payoffValue;
        private readonly Currency _currency;
        public DeterministicPayoff(double payoffValue, Currency currency) {
            _payoffValue = payoffValue;
            _currency = currency;
        }
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {    
            return _payoffValue;
        }

        public IEnumerable<Underlying> Dependencies => Enumerable.Empty<Underlying>();

        public Currency Currency => _currency;
    }
}
