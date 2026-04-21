using Domain;

namespace Application {
    public class DeterministicPayoff : INonPathDependentPayoff {
        private readonly double _payoffValue;
        public DeterministicPayoff(double payoffValue) {
            _payoffValue = payoffValue;
        }
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {    
            return _payoffValue;
        }

        public IReadOnlyList<Underlying> GetUnderlyingDependencyList() {
            return [];
        }
    }
}
