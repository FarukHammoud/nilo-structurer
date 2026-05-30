using Domain;

namespace Application {
    public class EuropeanPut : VanillaContract, IPut {
        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingPathIndependentPayoff(
                spot => Notional * Math.Max(0, Strike - spot), Underlying, Currency);
    }
}
