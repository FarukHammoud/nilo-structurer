using Domain;

namespace Application {
    public abstract class Package : IPathIndependentContract {
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs => 
            Contracts.SelectMany(c => c.PathIndependentPayoffs).ToList();

        public abstract List<IPathIndependentContract> Contracts { get; }
        public double Notional { get; set; } = 1.0;
    }
}
