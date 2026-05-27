using Domain;

namespace Application {
    public class AmericanCall : VanillaContract, ICall {
        public override INonPathDependentPayoff Payoff => new MonoUnderlyingNonPathDependentPayoff(
            spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency);
    }
}
