using Domain;

namespace Application {
    public class EuropeanPut : VanillaContract, IPut {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => Notional * Math.Max(0, Strike - spot), Underlying, Currency);
    }
}
