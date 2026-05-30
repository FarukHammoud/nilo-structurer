using Domain;

namespace Application {
    public class FactorPathIndependentPayoff : IPathIndependentPayoff {

        private IPathIndependentPayoff _basePayoff;
        private double _factor;

        public FactorPathIndependentPayoff(IPathIndependentPayoff basePayoff, double factor) {
            _basePayoff = basePayoff;
            _factor = factor;
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies;

        public Currency Currency => _basePayoff.Currency;

        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return _factor * _basePayoff.GetPayoffAtMaturity(pricesAtMaturity);
        }
    }
}
