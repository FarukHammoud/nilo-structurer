using Domain.Model.Payoffs;

namespace Domain {
    public class EuropeanCallOption : CallOption {
        public override IPayoff Payoff { get; set; }
        public EuropeanCallOption() {
            Payoff = new StatelessPayoff(
            spot => Math.Max(0, spot - Strike));
        }
    }
}
