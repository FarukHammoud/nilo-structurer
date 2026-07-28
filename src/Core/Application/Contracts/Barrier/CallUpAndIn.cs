using Domain;

namespace Application {
    public class CallUpAndIn : IContract {
        public IEnumerable<IFlow> Flows => [
            new UpAndInPayoff(
                new MonoUnderlyingPathIndependentPayoff() {
                    Payoff = spot => Math.Max(0, spot - Strike),
                    Underlying = Underlying,
                    Currency = Currency,
                    Maturity = Maturity,
                    PaymentDate = Maturity
                }, BarrierLevel, Underlying, StartDate)];
        public required Underlying Underlying { get; init; }
        public required Currency Currency { get; init; }
        public required double Strike { get; init; }
        public required double BarrierLevel { get; init; }
        public required DateTime StartDate { get; init; }
        public required DateTime Maturity { get; init; }
        public double Notional { get; init; } = 1;
    }
}
