namespace Domain {
    public interface IPathIndependentContract : IContract {
        IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs { get; }
        IEnumerable<IPayoff> IContract.Payoffs => PathIndependentPayoffs.Cast<IPayoff>();
        IEnumerable<DateTime> IContract.Dates =>
            Payoffs.Select(p => p.Maturity)
               .Distinct()
               .Order();
    }
}
