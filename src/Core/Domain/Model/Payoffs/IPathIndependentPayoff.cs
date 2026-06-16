namespace Domain {
    public interface IPathIndependentPayoff : IPayoff {
        DateTime Maturity { get; init; }

        // Simpler Compute Payoff
        double ComputePayoff(Dictionary<Underlying, double> pricesAtMaturity);

        // Auto-implemented IPayoff Attributes / Methods
        MonitoringFrequency IPayoff.MonitoringFrequency => MonitoringFrequency.None;
        IReadOnlyList<DateTime> IPayoff.ObservationDates => [Maturity];
        double IPayoff.ComputePayoff(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            return ComputePayoff(prices[Maturity]);
        }
    }
}
