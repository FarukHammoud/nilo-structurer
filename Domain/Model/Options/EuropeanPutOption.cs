using Domain.Model.Payoffs;

namespace Domain {
    public class EuropeanPutOption : PutOption {
        public override IPayoff Payoff { get; set; }
        public EuropeanPutOption() {
            Payoff = new StatelessPayoff(
            spot => Math.Max(0, spot - Strike));
        }
    }
}
