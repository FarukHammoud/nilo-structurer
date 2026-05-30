using Domain;

namespace Application {
    public class ProductPathIndependentPayoff : IPathIndependentPayoff {

        private IEnumerable<IPathIndependentPayoff> _payoffs;
        private Currency _currency;

        public ProductPathIndependentPayoff(IEnumerable<IPathIndependentPayoff> payoffs, Currency currency) {
            _payoffs = payoffs;
            _currency = currency;
        }
         
        public double ComputePayoff(Dictionary<Underlying, double> pricesAtMaturity) {
            return _payoffs.Aggregate(1.0, (product, payoff) => product * payoff.ComputePayoff(pricesAtMaturity));
        }

        public IEnumerable<Underlying> Dependencies => _payoffs.SelectMany(payoff => payoff.Dependencies).Distinct();

        public Currency Currency => _currency;
        public DateTime Maturity { get; init; }
        public DateTime PaymentDate { get; init; }
    }
}
