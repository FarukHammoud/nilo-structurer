namespace Domain {
    public interface IPathDependentContract : IGeneralContract<IPathDependentPayoff> {
        IEnumerable<DateTime> IContract.Dates =>
        Payoffs.SelectMany(p => p.ObservationDates)
               .Distinct()
               .Order()
               .ToList();
    }
}
