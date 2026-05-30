using Domain;

namespace Application {
    public class QuantoEuropeanCall : QuantoVanilla, ICall {

        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingQuantoPathIndependentPayoff(
                spot => Notional * Math.Max(0, spot - Strike), Underlying, Currency, FxRate);
    }
}
