using Domain;

namespace Application {
    public class QuantoEuropeanPut : QuantoVanilla, IPut {

        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingQuantoPathIndependentPayoff() {
                Payoff = spot => Notional * Math.Max(0, Strike - spot),
                Underlying = Underlying,
                Currency = Currency,
                FixedFxRate = FxRate,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
