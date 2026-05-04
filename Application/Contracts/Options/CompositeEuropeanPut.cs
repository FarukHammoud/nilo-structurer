using Domain;

namespace Application {
    public class CompositeEuropeanPut : VanillaContract, IPut {

        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingCompositeNonPathDependentPayoff(
                spot => Notional * Math.Max(0, Strike - spot), Underlying, Currency);
    }
}
