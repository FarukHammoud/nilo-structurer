using Domain;

namespace Application {
    public abstract class Package : IPathIndependentContract {
        public IEnumerable<IPathIndependentPayoff> Payoffs => 
            Contracts.SelectMany(c => c.Payoffs).ToList();

        public abstract List<IPathIndependentContract> Contracts { get; }
        public double Notional { get; set; } = 1.0;
    }
}
