using Domain;

namespace Application {
    public class CompositeForward : Forward {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingCompositeNonPathDependentPayoff(
                spot => Notional * (spot - Strike), Underlying, Currency);
    }
}
