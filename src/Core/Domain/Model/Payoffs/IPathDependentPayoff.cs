namespace Domain {
    /// <summary>
    /// Base contract for any path-dependent payoff.
    /// </summary>
    public interface IPathDependentPayoff : IPayoff {
        double GetPayoffAtMaturity(Dictionary<DateTime, Dictionary<Underlying, double>> prices);
        List<DateTime> GetObservationDates();
        MonitoringFrequency MonitoringFrequency { get; }
    }
}
