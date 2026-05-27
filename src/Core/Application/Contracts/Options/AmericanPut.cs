using Domain;

namespace Application {
    public class AmericanPut : VanillaContract,IPut {
        public override INonPathDependentPayoff Payoff => new MonoUnderlyingNonPathDependentPayoff(
            spot => Notional * Math.Max(0, Strike - spot), Underlying, Currency);
    }
}
