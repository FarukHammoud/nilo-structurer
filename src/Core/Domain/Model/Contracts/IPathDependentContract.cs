namespace Domain {
    public interface IPathDependentContract : IContract {
        IEnumerable<IPathDependentPayoff> PathDependentPayoffs { get; }
        IEnumerable<IPayoff> IContract.Payoffs => PathDependentPayoffs.Cast<IPayoff>();
        IEnumerable<DateTime> IContract.Dates =>
        Payoffs.SelectMany(p => p.ObservationDates)
               .Distinct()
               .Order()
               .ToList();
    }
}
