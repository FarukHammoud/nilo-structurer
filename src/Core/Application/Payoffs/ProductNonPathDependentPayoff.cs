using Domain;

namespace Application {
    public class ProductNonPathDependentPayoff : INonPathDependentPayoff {

        private IEnumerable<INonPathDependentPayoff> _payoffs;
        private Currency _currency;

        public ProductNonPathDependentPayoff(IEnumerable<INonPathDependentPayoff> payoffs, Currency currency) {
            _payoffs = payoffs;
            _currency = currency;
        }
         
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return _payoffs.Aggregate(1.0, (product, payoff) => product * payoff.GetPayoffAtMaturity(pricesAtMaturity));
        }

        public IEnumerable<Underlying> Dependencies => _payoffs.SelectMany(payoff => payoff.Dependencies).Distinct();

        public Currency Currency => _currency;
    }
}
