namespace Domain {
    /// <summary>
    /// Implements the Actual/360 day count convention.
    /// </summary>
    public sealed class Actual360 : IDayCountConvention {
        public double YearFraction(DateTime start, DateTime end)
            => (end - start).Days / 360.0;
    }
}
