using Domain;

namespace Application {
    public class BinaryPut : VanillaContract, IPut {
        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingPathIndependentPayoff() {
                Payoff = spot => spot < Strike ? Notional : 0,
                Underlying = Underlying,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
