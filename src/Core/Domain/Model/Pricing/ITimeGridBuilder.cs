namespace Domain {
    public interface ITimeGridBuilder {
        IEnumerable<DateTime> Build(IEnumerable<IContract> contracts, ModelConfiguration model, DateTime valuationDate);
    }
}
