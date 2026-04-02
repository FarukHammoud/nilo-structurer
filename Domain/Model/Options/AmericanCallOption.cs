using Domain.Model.Payoffs;

namespace Domain {
    public class AmericanCallOption : CallOption {
        public IPayoff Payoff { get; set; }
        public AmericanCallOption() {
            Payoff = new StatelessPayoff(
            spot => Math.Max(0, spot - Strike));
        }
    }
}
