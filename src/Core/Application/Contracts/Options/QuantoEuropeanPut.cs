using Domain;

namespace Application {
    public class QuantoEuropeanPut : QuantoVanilla, IPut {

        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingQuantoNonPathDependentPayoff(
                spot => Notional * Math.Max(0, Strike - spot), Underlying, Currency, FxRate);
    }
}
