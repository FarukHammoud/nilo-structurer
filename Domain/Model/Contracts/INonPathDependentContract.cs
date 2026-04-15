using System.Collections.Specialized;

namespace Domain {
    public interface INonPathDependentContract : IContract {
        IEnumerable<Tuple<DateTime, INonPathDependentPayoff>> Payoffs { get; }
    }
}
