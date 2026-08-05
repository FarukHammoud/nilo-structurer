namespace Domain {
    /// <summary>
    /// Defines the interface for day count conventions used in accrual/discount calculations.
    /// </summary>
    public interface IDayCountConvention {
        double YearFraction(DateTime start, DateTime end);
    }
}
