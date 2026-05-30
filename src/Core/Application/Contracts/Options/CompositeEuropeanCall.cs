using Domain;

namespace Application {
    public class CompositeEuropeanCall : VanillaContract, ICall {

        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingCompositePathIndependentPayoff() {
                Payoff = spot => Notional * Math.Max(0, spot - Strike),
                Underlying = Underlying,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
