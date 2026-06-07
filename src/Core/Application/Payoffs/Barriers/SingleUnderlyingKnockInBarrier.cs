using Domain;

namespace Application {
    public class SingleUnderlyingKnockInBarrier : IKnockInBarrier {
        public required Underlying Underlying { get; init; }
        public required double BarrierLevel { get; init; }
        public MonitoringFrequency MonitoringFrequency { get; init; } = MonitoringFrequency.Continuous;

        public required IReadOnlyList<DateTime> ObservationDates { get; init; } 

        public required IPayoff ActivatedPayoff { get; init; }
        public required Func<Dictionary<DateTime, double>, bool> IsTouched { get; init; }

        public bool IsTriggered(Dictionary<DateTime, double> path) {
            return IsTriggered(path);
        }
    }
}
