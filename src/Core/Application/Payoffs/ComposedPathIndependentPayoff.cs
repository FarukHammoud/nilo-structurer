using Domain;

namespace Application {
    public class ComposedPathIndependentPayoff : IPathIndependentPayoff {

        private List<IPathIndependentPayoff> _payoffs;
        public Currency Currency => _payoffs.First().Currency; // Thats why we should delete this one

        public ComposedPathIndependentPayoff(List<IPathIndependentPayoff> payoffs) {
            _payoffs = payoffs;
        }

        public double ComputePayoff(Dictionary<Underlying, double> pricesAtMaturity) {
            return _payoffs.Sum(payoff => payoff.ComputePayoff(pricesAtMaturity));
        }

        public IEnumerable<Underlying> Dependencies =>
            _payoffs.SelectMany(payoff => payoff.Dependencies).Distinct();

        public DateTime Maturity { get; init; }
        public DateTime PaymentDate { get; init; }
    }
}
