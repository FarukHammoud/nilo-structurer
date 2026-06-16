using Domain;

namespace Application {
    public class MonoUnderlyingPathDependentPayoff : IPathDependentPayoff {
        public required Func<Dictionary<DateTime, double>, double> PayoffMap { get; init; }
        public required Underlying Underlying { get; init; }
        public required Currency Currency { get; init; }
        public required MonitoringFrequency MonitoringFrequency { get; init; }

        public double ComputePayoff(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> underlyingValues = Underlying.GetValues(prices);
            return PayoffMap(underlyingValues);
        }

        public IEnumerable<Underlying> Dependencies => Underlying.Dependencies;

        public required IReadOnlyList<DateTime> ObservationDates { get; init; }

        public required DateTime PaymentDate { get; init; }
        public required DateTime Maturity { get; init; }
    }
}
