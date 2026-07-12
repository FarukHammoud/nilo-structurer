using Domain;

namespace Application {
    public class SingleUnderlyingKnockInBarrier : IKnockInBarrier {
        public required Underlying Underlying { get; init; }
        public required double BarrierLevel { get; init; }
        public MonitoringFrequency MonitoringFrequency { get; init; } = MonitoringFrequency.Continuous;

        public required IReadOnlyList<DateTime> ObservationDates { get; init; } 

        public required IPayoff ActivatedPayoff { get; init; }
        public required Func<SimulatedPath, bool> IsTouched { get; init; }

        public bool IsTriggered(SimulatedPath path) {
            return IsTriggered(path);
        }
    }
}
