using Domain;

namespace PricerServices {
    public interface ITimeGridBuilder {
        IEnumerable<DateTime> Build(IEnumerable<IContract> observationDates, DateTime valuationDate);
    }
}
