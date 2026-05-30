using Domain;

namespace Application {
    public abstract class PathDependentPackage : IPathDependentContract {
        public IEnumerable<IPathDependentPayoff> Payoffs => 
            Contracts.SelectMany(c => c.Payoffs);

        public abstract IEnumerable<IPathDependentContract> Contracts { get; }
        public double Notional { get; set; } = 1.0;
    }
}
