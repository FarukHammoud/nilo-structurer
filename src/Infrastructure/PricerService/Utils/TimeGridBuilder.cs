using Domain;

namespace PricerServices {
    public class TimeGridBuilder : ITimeGridBuilder {
        public IEnumerable<DateTime> Build(IEnumerable<IContract> contracts, DateTime valuationDate) {
            List<DateTime> dates = contracts.SelectMany(contract => contract.Dates)
                .Where(date => date > valuationDate)
                .Append(valuationDate)
                .Distinct()
                .OrderBy(date => date)
                .ToList();
            bool pathDependent = contracts.Any(c => c is IPathDependentContract);

            if (!pathDependent) {
                return dates;
            }
            return Enumerable.Range(0, (int)(dates.Max() - dates.Min()).TotalDays + 1)
                .Select(i => dates.Min().AddDays(i))
                .ToList();
        }
    }
}
