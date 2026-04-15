namespace Domain {
    public interface IPathDependentContract : IContract {
        IEnumerable<Tuple<DateTime, IPathDependentPayoff>> Payoffs { get; }
    }
}
