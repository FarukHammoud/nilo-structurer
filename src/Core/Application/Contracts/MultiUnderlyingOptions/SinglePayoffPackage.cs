using Domain;

namespace Application {
    public abstract class SinglePayoffPackage : Package {
        public abstract List<SinglePayoffNonPathDependentContract> SinglePayoffContracts { get; }
        public override List<INonPathDependentContract> Contracts => SinglePayoffContracts.Cast<INonPathDependentContract>().ToList();
        public INonPathDependentPayoff Payoff => new ComposedNonPathDependentPayoff(SinglePayoffContracts.Select(x => x.Payoff).ToList());
        public DateTime Maturity { get; set; }
    }
}
