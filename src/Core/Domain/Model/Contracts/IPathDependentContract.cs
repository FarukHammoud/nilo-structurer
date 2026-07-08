namespace Domain {
    public interface IPathDependentContract : IContract {
        IEnumerable<IPathDependentPayoff> PathDependentPayoffs { get; }
        IEnumerable<IFlow> IContract.Flows => PathDependentPayoffs.Cast<IFlow>();
    }
}
