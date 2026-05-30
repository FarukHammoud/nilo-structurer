using Domain;

namespace Application {
    public class MonoUnderlyingPathIndependentPayoff : IPathIndependentPayoff {
        public required Func<double, double> Payoff { get; init; }
        public required Underlying Underlying { get; init; }
        public required Currency Currency { get; init; }
        public required DateTime Maturity { get; init; }
        public required DateTime PaymentDate { get; init; }

        public IEnumerable<Underlying> Dependencies => Underlying.Dependencies;

        public double ComputePayoff(Dictionary<Underlying, double> pricesAtMaturity) {    
            double underlyingValue = Underlying.GetValue(pricesAtMaturity);
            return Payoff(underlyingValue);
        }
    }
}
