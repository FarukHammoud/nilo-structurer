namespace Domain {
    public class ComposedNonPathDependentPayoff : INonPathDependentPayoff {

        private List<INonPathDependentPayoff> _payoffs;

        public ComposedNonPathDependentPayoff(List<INonPathDependentPayoff> payoffs) {
            _payoffs = payoffs;
        }
         
        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return _payoffs.Sum(payoff => payoff.GetPayoffAtMaturity(pricesAtMaturity));
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return _payoffs.SelectMany(payoff => payoff.GetUnderlyingDependencyList()).Distinct().ToList();
        }
    }
}
