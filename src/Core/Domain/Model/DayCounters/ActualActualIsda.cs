namespace Domain {
    /// <summary>
    /// Implements the Actual/Actual (ISDA) day count convention.
    /// Considers different year lengths (365 or 366 days) for each year in the period.
    /// </summary>
    public sealed class ActualActualIsda : IDayCountConvention {
        public double YearFraction(DateTime start, DateTime end) {
            int startYear = start.Year;
            int endYear = end.Year;
            double yearFraction = 0.0;
            for (int year = startYear; year <= endYear; year++) {
                DateTime yearStart = new DateTime(year, 1, 1);
                DateTime yearEnd = new DateTime(year, 12, 31);
                DateTime periodStart = (year == startYear) ? start : yearStart;
                DateTime periodEnd = (year == endYear) ? end : yearEnd;
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                int daysInPeriod = (periodEnd - periodStart).Days + 1;
                yearFraction += (double)daysInPeriod / daysInYear;
            }
            return yearFraction;
        }
    }
}
