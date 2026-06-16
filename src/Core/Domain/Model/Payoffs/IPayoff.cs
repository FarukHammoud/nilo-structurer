namespace Domain {
    public interface IPayoff {
        double ComputePayoff(Dictionary<DateTime, Dictionary<Underlying, double>> prices);
        IReadOnlyList<DateTime> ObservationDates { get; }
        MonitoringFrequency MonitoringFrequency { get; }
        IEnumerable<Underlying> Dependencies { get; }
        DateTime Maturity { get; }
        DateTime PaymentDate { get; }
        Currency Currency { get; }
    }
}
