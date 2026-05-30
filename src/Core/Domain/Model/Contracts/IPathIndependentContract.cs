namespace Domain {
    public interface IPathIndependentContract : IGeneralContract<IPathIndependentPayoff> {
        IEnumerable<DateTime> IContract.Dates =>
            Payoffs.Select(p => p.Maturity)
               .Distinct()
               .Order();
    }
}
