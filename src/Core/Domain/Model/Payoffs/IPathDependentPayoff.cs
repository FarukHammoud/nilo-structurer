namespace Domain {
    /// <summary>
    /// Base contract for any path-dependent payoff.
    /// </summary>
    public interface IPathDependentPayoff : IPayoff {
        double ComputePayoff(Dictionary<DateTime, Dictionary<Underlying, double>> prices);
        IReadOnlyList<DateTime> ObservationDates { get; }
        MonitoringFrequency MonitoringFrequency { get; }
    }
}
