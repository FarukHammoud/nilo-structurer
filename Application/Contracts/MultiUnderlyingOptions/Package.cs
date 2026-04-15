using Domain;

namespace Application {
    public abstract class Package : INonPathDependentContract {
        public IEnumerable<Tuple<DateTime, INonPathDependentPayoff>> Payoffs => 
            Contracts.SelectMany(c => c.Payoffs).ToList();

        public abstract List<INonPathDependentContract> Contracts { get; }
        public double Notional { get; set; } = 1.0;
    }
}
