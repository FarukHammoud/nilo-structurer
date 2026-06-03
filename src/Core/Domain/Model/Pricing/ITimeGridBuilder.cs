using Domain;

namespace Domain {
    public interface ITimeGridBuilder {
        IEnumerable<DateTime> Build(IEnumerable<IContract> observationDates, DateTime valuationDate);
    }
}
