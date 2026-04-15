using Domain;

namespace Application {
    public class ProductNonPathDependentPayoff : INonPathDependentPayoff {

        private List<INonPathDependentPayoff> _payoffs;

        public ProductNonPathDependentPayoff(List<INonPathDependentPayoff> payoffs) {
            _payoffs = payoffs;
        }
         
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return _payoffs.Aggregate(1.0, (product, payoff) => product * payoff.GetPayoffAtMaturity(pricesAtMaturity));
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return _payoffs.SelectMany(payoff => payoff.GetUnderlyingDependencyList()).Distinct().ToList();
        }
    }
}
