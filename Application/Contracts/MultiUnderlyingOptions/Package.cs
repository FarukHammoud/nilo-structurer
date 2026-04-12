using Domain;

namespace Application {
    public abstract class Package : NonPathDependentContract {
        public override INonPathDependentPayoff Payoff => new ComposedNonPathDependentPayoff(Contracts.Select(x => x.Payoff).ToList());
        public abstract List<NonPathDependentContract> Contracts { get; }
    }
}
