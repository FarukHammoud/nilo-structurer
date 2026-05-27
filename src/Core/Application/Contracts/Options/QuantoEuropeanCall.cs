using Domain;

namespace Application {
    public class QuantoEuropeanCall : QuantoVanilla, ICall {

        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingQuantoNonPathDependentPayoff(
                spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency, FxRate);
    }
}
