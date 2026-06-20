using Domain;

namespace Application {
    public class FeeOnPremium : SinglePayoffPathIndependentContract {
        public required SinglePayoffPathIndependentContract Reference { get; set; }
        public required double Fee { get; set; }
        public override IPathIndependentPayoff Payoff => new FactorPathIndependentPayoff(Reference.Payoff, Fee);
    }
}
