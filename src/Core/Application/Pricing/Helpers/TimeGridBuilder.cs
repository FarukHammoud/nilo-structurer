using Domain;

namespace Application {
    public class TimeGridBuilder : ITimeGridBuilder {
        public IList<DateTime> Build(IEnumerable<IContract> contracts, ModelConfiguration model, DateTime valuationDate) {
            List<DateTime> dates = contracts.SelectMany(contract => contract.Dates)
                .Where(date => date > valuationDate)
                .Append(valuationDate)
                .Distinct()
                .OrderBy(date => date)
                .ToList();
            bool pathIndependent = !contracts.Any(c => c is IPathDependentContract);

            if (pathIndependent) {
                if (model.Discounting is StochasticRatesDiscounting) {
                    return DailyGrid(dates);
                }
                if (model.Pricing is DermanKani) {
                    return MonthlyGrid(dates);
                }
                return dates;
            }
            return DailyGrid(dates);
        }

        public static Func<IEnumerable<DateTime>, List<DateTime>> DailyGrid = dates => Enumerable.Range(0, (int)(dates.Max() - dates.Min()).TotalDays + 1)
                .Select(i => dates.Min().AddDays(i))
                .ToList();

        public static Func<IEnumerable<DateTime>, List<DateTime>> WeeklyGrid = dates => Enumerable.Range(0, (int)((dates.Max() - dates.Min()).TotalDays / 7) + 1)
                .Select(i => dates.Min().AddDays(i * 7))
                .Append(dates.Max())
                .Distinct()
                .ToList();

        public static Func<IEnumerable<DateTime>, List<DateTime>> MonthlyGrid = dates => Enumerable.Range(0, (int)((dates.Max() - dates.Min()).TotalDays / 30) + 1)
                .Select(i => dates.Min().AddMonths(i))
                .Append(dates.Max())
                .Distinct()
                .ToList();

        public static Func<IEnumerable<DateTime>, List<DateTime>> BiMonthlyGrid = dates => Enumerable.Range(0, (int)((dates.Max() - dates.Min()).TotalDays / 60) + 1)
                .Select(i => dates.Min().AddMonths(i))
                .Append(dates.Max())
                .Distinct()
                .ToList();
    }
}
