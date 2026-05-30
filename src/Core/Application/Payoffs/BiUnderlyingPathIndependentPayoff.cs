using Domain;

namespace Application {
    public class BiUnderlyingPathIndependentPayoff : IPathIndependentPayoff {
        public required Func<double, double, double> Payoff { get; init; }
        public required Underlying FirstUnderlying { get; init; }
        public required Underlying SecondUnderlying { get; init; }
        public required Currency Currency { get; init; }
        public required DateTime PaymentDate { get; init; }
        public required DateTime Maturity { get; init; }

        public double ComputePayoff(Dictionary<Underlying, double> pricesAtMaturity) {
            double firstUnderlyingValue = FirstUnderlying.GetValue(pricesAtMaturity);
            double secondUnderlyingValue = SecondUnderlying.GetValue(pricesAtMaturity);
            return Payoff(firstUnderlyingValue, secondUnderlyingValue);
        }

        public IEnumerable<Underlying> Dependencies =>
                FirstUnderlying.Dependencies
            .Union(SecondUnderlying.Dependencies);
    }
}
