namespace Domain {
    public interface IKnockInBarrier {
        IReadOnlyList<DateTime> ObservationDates { get; }
        MonitoringFrequency MonitoringFrequency { get; }
        bool IsTriggered(IPricePath path, DateTime observationDate);
        IPathDependentPayoff ActivatedPayoff { get; }
    }
}
