using Domain;

namespace Application {
    public class ComposedNonPathDependentPayoff : INonPathDependentPayoff {

        private List<INonPathDependentPayoff> _payoffs;
        public Currency Currency => _payoffs.First().Currency; // Thats why we should delete this one

        public ComposedNonPathDependentPayoff(List<INonPathDependentPayoff> payoffs) {
            _payoffs = payoffs;
        }
         
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return _payoffs.Sum(payoff => payoff.GetPayoffAtMaturity(pricesAtMaturity));
        }

        public IEnumerable<Underlying> Dependencies =>
            _payoffs.SelectMany(payoff => payoff.Dependencies).Distinct();
        
    }
}
