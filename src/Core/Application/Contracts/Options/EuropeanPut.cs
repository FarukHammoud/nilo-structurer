using Domain;

namespace Application {
    public class EuropeanPut : VanillaContract, IPut {
        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingPathIndependentPayoff() {
                Payoff = spot => Notional * Math.Max(0, Strike - spot),
                Underlying = Underlying,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };}
}
