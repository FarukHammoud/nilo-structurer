namespace Domain {
    public interface IPayoff : IFlow {
        double ComputePayoff(Scenario scenario);
        IReadOnlyList<DateTime> ObservationDates { get; } // Maybe Fixing Dates?
        MonitoringFrequency MonitoringFrequency { get; }
        IEnumerable<Underlying> Dependencies { get; }
        DateTime Maturity { get; } // Maybe observation date
        DateTime PaymentDate { get; }
        Currency Currency { get; }
        DateTime IFlow.Date => Maturity; 
    }
}
