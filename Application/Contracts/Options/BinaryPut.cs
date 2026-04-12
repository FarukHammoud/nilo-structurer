using Domain;

namespace Application {
    public class BinaryPut : VanillaContract {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => spot < Strike ? Notional : 0, Underlying);
    }
}
