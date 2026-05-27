using Domain;

namespace Application {
    public class CompositeEuropeanCall : VanillaContract, ICall {

        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingCompositeNonPathDependentPayoff(
                spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency);
    }
}
