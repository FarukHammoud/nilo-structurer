namespace Domain {
    public interface ITimeGridBuilder {
        IList<DateTime> Build(IEnumerable<IContract> contracts, ModelConfiguration model, DateTime valuationDate);
    }
}
