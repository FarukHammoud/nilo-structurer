using Domain;

namespace Application {
    public class CompositeEuropeanCall : VanillaContract, ICall {

        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingCompositePathIndependentPayoff(
                spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency);
    }
}
