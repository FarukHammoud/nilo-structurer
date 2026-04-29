using Domain;

namespace Application {
    public class EuropeanCall : Call {

        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency);

    }
}
