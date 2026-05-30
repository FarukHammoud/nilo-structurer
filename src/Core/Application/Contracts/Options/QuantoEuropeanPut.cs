using Domain;

namespace Application {
    public class QuantoEuropeanPut : QuantoVanilla, IPut {

        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingQuantoPathIndependentPayoff(
                spot => Notional * Math.Max(0, Strike - spot), Underlying, Currency, FxRate);
    }
}
