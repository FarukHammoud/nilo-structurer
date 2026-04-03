using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain {
    public class BinaryPut : VanillaContract {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => spot < Strike ? Notional : 0, Underlying);
    }
}
