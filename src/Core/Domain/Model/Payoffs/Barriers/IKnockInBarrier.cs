namespace Domain {
    public interface IKnockInBarrier {
        IReadOnlyList<DateTime> ObservationDates { get; }
        MonitoringFrequency MonitoringFrequency { get; }
        bool IsTriggered(Dictionary<DateTime, double> path);
        IPayoff ActivatedPayoff { get; }
    }
}
