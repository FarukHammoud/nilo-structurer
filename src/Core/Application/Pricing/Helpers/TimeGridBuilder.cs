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
                    return _dailyGrid(dates);
                }
                return dates;
            }
            return _dailyGrid(dates);
        }

        private Func<List<DateTime>, List<DateTime>> _dailyGrid = dates => Enumerable.Range(0, (int)(dates.Max() - dates.Min()).TotalDays + 1)
                .Select(i => dates.Min().AddDays(i))
                .ToList();

        private Func<List<DateTime>, List<DateTime>> _weeklyGrid = dates => Enumerable.Range(0, (int)((dates.Max() - dates.Min()).TotalDays / 7) + 1)
                .Select(i => dates.Min().AddDays(i * 7))
                .Append(dates.Max())
                .Distinct()
                .ToList();
    }
}
