using Domain.Model.Payoffs;

namespace Domain {
    public class AmericanPutOption : PutOption {
        public override IPayoff Payoff { get; set; }
        public AmericanPutOption() {
            Payoff = new StatelessPayoff(
            spot => Math.Max(0, Strike - spot));
        }
    }
}
