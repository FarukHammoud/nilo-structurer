namespace Domain {
    /// <summary>
    /// Implements the Actual/365 day count convention.
    /// </summary>
    public sealed class Actual365 : IDayCountConvention {
        public double YearFraction(DateTime start, DateTime end)
            => (end - start).Days / 365.0;
    }
}
