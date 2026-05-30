using Domain;

namespace Application {
    public class CompositeForward : Forward {
        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingCompositePathIndependentPayoff() {
                Payoff = spot => Notional * (spot - Strike),
                Underlying = Underlying,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
