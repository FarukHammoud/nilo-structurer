using Domain;

namespace Application {
    public class QuantoConverse : SinglePayoffPathIndependentContract {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required double FxRate { get; set; }
        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingQuantoPathIndependentPayoff() {
                Payoff = spot => Notional * (spot - Strike),
                Underlying = Underlying,
                Currency = Currency,
                FixedFxRate = FxRate,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
    }
}
