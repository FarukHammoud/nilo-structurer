using Domain;

namespace Application {
    public class EuropeanCall : VanillaContract, ICall {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency);

    }
}
