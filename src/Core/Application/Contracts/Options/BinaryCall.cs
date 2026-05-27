using Domain;

namespace Application {
    public class BinaryCall : VanillaContract, ICall {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => spot > Strike ? Notional : 0, Underlying, Currency);
    }
}
