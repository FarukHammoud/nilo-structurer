using Domain;

namespace Application {
    public class BinaryPut : VanillaContract, IPut {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => spot < Strike ? Notional : 0, Underlying, Currency);
    }
}
