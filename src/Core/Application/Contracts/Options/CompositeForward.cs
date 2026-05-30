using Domain;

namespace Application {
    public class CompositeForward : Forward {
        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingCompositePathIndependentPayoff(
                spot => Notional * (spot - Strike), Underlying, Currency);
    }
}
