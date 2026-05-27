using Domain;

namespace Application {
    public class FeeOnPremium : SinglePayoffNonPathDependentContract {
        public required SinglePayoffNonPathDependentContract Reference { get; set; }
        public required double Fee { get; set; }
        public override INonPathDependentPayoff Payoff => new FactorNonPathDependentPayoff(Reference.Payoff, Fee);
    }
}
