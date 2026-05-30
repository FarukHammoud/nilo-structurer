using Domain;

namespace Application {
    public class BinaryCall : VanillaContract, ICall {
        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingPathIndependentPayoff(
                spot => spot > Strike ? Notional : 0, Underlying, Currency);
    }
}
