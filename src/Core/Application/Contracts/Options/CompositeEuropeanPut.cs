using Domain;

namespace Application {
    public class CompositeEuropeanPut : VanillaContract, IPut {

        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingCompositePathIndependentPayoff(
                spot => Notional * Math.Max(0, Strike - spot), Underlying, Currency);
    }
}
