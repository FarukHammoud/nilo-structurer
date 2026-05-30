using Domain;

namespace Application {
    public class CompositeEuropeanPut : VanillaContract, IPut {

        public override IPathIndependentPayoff Payoff => 
            new MonoUnderlyingCompositePathIndependentPayoff() {
                Payoff = spot => Notional * Math.Max(0, Strike - spot),
                Underlying = Underlying,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
