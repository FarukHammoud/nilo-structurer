using System.Collections.Specialized;

namespace Domain {
    public interface IPathIndependentContract : IContract {
        IEnumerable<Tuple<DateTime, IPathIndependentPayoff>> Payoffs { get; }
    }
}
