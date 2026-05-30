using Domain;

namespace Application {
    public class BinaryPut : VanillaContract, IPut {
        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingPathIndependentPayoff(
                spot => spot < Strike ? Notional : 0, Underlying, Currency);
    }
}
