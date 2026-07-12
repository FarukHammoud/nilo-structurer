namespace Domain {
    public interface IKnockInBarrier {
        IReadOnlyList<DateTime> ObservationDates { get; }
        MonitoringFrequency MonitoringFrequency { get; }
        bool IsTriggered(SimulatedPath path);
        IPayoff ActivatedPayoff { get; }
    }
}
