namespace Domain {
    public interface IPathIndependentContract : IContract {
        IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs { get; }
        IEnumerable<IFlow> IContract.Flows => PathIndependentPayoffs.Cast<IFlow>();
    }
}
