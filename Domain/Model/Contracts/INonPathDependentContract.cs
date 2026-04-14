using System.Collections.Specialized;

namespace Domain {
    public interface INonPathDependentContract : IContract {
        List<Tuple<DateTime, INonPathDependentPayoff>> Payoffs { get; }
    }
}
