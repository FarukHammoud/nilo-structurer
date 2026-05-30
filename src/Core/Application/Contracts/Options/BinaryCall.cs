using Domain;

namespace Application {
    public class BinaryCall : VanillaContract, ICall {
        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingPathIndependentPayoff() {
                Payoff = spot => spot > Strike ? Notional : 0,
                Underlying = Underlying,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
