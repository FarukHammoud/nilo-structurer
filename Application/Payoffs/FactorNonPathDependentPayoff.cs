using Domain;

namespace Application {
    public class FactorNonPathDependentPayoff : INonPathDependentPayoff {

        private INonPathDependentPayoff _basePayoff;
        private double _factor;

        public FactorNonPathDependentPayoff(INonPathDependentPayoff basePayoff, double factor) {
            _basePayoff = basePayoff;
            _factor = factor;
        }
         
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return _factor * _basePayoff.GetPayoffAtMaturity(pricesAtMaturity);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return _basePayoff.GetUnderlyingDependencyList();
        }
    }
}
