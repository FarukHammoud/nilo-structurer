using Domain;

namespace Application {
    public class SingleUnderlyingUpAndInBarrier : IKnockInBarrier {
        public required Underlying Underlying { get; init; }
        public required double BarrierLevel { get; init; }
        public MonitoringFrequency MonitoringFrequency { get; init; } = MonitoringFrequency.Continuous;

        public required IReadOnlyList<DateTime> ObservationDates { get; init; } 

        public required IPayoff ActivatedPayoff { get; init; }

        public bool IsTriggered(IPricePath path, DateTime observationDate) {
            return path.GetValue(observationDate, Underlying) > BarrierLevel;
        }
    }
}
