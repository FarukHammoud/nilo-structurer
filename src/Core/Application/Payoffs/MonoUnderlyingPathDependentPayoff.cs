using Domain;

namespace Application {
    public class MonoUnderlyingPathDependentPayoff : IPathDependentPayoff {
        public required Func<Dictionary<DateTime, double>, double> PayoffMap { get; init; }
        public required Underlying Underlying { get; init; }
        public required Currency Currency { get; init; }
        public required MonitoringFrequency MonitoringFrequency { get; init; }

        public double ComputePayoff(Scenario scenario) {
            SimulatedPath path = scenario[Underlying];
            Dictionary<DateTime, double> pathValues = Enumerable.Range(0, scenario.Dates.Count)
                .ToDictionary(i => scenario.Dates[i], i => path.Values[i]);
            return PayoffMap(pathValues);
        }

        public IEnumerable<Underlying> Dependencies => Underlying.Dependencies;

        public required IReadOnlyList<DateTime> ObservationDates { get; init; }

        public required DateTime PaymentDate { get; init; }
        public required DateTime Maturity { get; init; }
    }
}
