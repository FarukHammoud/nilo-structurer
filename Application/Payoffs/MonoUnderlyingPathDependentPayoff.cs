using Domain;

namespace Application {
    public class MonoUnderlyingPathDependentPayoff : IPathDependentPayoff {
        public required Func<Dictionary<DateTime, double>, double> PayoffMap { get; init; }
        public required Underlying Underlying { get; init; }
        public required List<DateTime> ObservationDates { get; init; }
        public required MonitoringFrequency MonitoringFrequency { get; init; }

        public MonitoringFrequency GetMonitoringFrequency() {
            return MonitoringFrequency;
        }

        public List<DateTime> GetObservationDates() {
            return ObservationDates;
        }

        public double GetPayoffAtMaturity(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> underlyingValues = ((IPathDependentPayoff)this).GetUnderlyingValues(Underlying, prices);
            return PayoffMap(underlyingValues);
        }

        public IEnumerable<Underlying> Dependencies => Underlying.Dependencies;
    }
}
