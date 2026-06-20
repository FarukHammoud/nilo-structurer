using Domain;

namespace Application {
    public abstract class SinglePayoffPackage : Package {
        public abstract List<SinglePayoffPathIndependentContract> SinglePayoffContracts { get; }
        public override List<IPathIndependentContract> Contracts => SinglePayoffContracts.Cast<IPathIndependentContract>().ToList();
        public IPathIndependentPayoff Payoff => new ComposedPathIndependentPayoff(SinglePayoffContracts.Select(x => x.Payoff).ToList());
        public DateTime Maturity { get; set; }
    }
}
