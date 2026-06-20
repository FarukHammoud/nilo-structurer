using Domain;

namespace Application {
    public class EuropeanCall : VanillaContract, ICall {
        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingPathIndependentPayoff() {
                Payoff = spot => Notional * Math.Max(0, spot - Strike),
                Underlying = Underlying,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
