using Domain;

namespace Application {
    public abstract class PathDependentPackage : IPathDependentContract {
        public IEnumerable<IPathDependentPayoff> PathDependentPayoffs => 
            Contracts.SelectMany(c => c.PathDependentPayoffs);

        public abstract IEnumerable<IPathDependentContract> Contracts { get; }
        public double Notional { get; set; } = 1.0;
    }
}
