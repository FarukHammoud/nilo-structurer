using Domain;

namespace Application {
    public class DeterministicPayoff : IPathIndependentPayoff {
        public required double PayoffValue { get; init; }
        public required Currency Currency { get; init; }
        public required DateTime Maturity { get; init; }
        public required DateTime PaymentDate { get; init; }
        public IEnumerable<Underlying> Dependencies => Enumerable.Empty<Underlying>();
        public double ComputePayoff(Dictionary<Underlying, double> _) {    
            return PayoffValue;
        }
    }
}
