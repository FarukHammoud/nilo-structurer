using Domain;

namespace Application {
    public class QuantoEuropeanCall : QuantoVanilla, ICall {

        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingQuantoPathIndependentPayoff() {
                Payoff = spot => Notional * Math.Max(0, spot - Strike),
                Underlying = Underlying,
                Currency = Currency,
                FixedFxRate = FxRate,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
