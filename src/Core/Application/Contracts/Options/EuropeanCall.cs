using Domain;

namespace Application {
    public class EuropeanCall : VanillaContract, ICall {
        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingPathIndependentPayoff(
                spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency);

    }
}
