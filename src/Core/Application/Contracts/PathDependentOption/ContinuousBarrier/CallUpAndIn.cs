using Domain;

namespace Application {
    public class CallUpAndIn : IPathDependentContract {
        public IEnumerable<IPathDependentPayoff> PathDependentPayoffs => [
            new UpAndInPayoff(
                new MonoUnderlyingPathDependentPayoff() {
                    PayoffMap = d => Math.Max(0, d.Values.Last() - Strike),
                    ObservationDates = [Maturity],
                    Underlying = Underlying,
                    MonitoringFrequency = MonitoringFrequency.Continuous,
                    Currency = Currency,
                    Maturity = Maturity,
                    PaymentDate = Maturity
                }, BarrierLevel, Underlying)];
        public required Underlying Underlying { get; init; }
        public required Currency Currency { get; init; }
        public required double Strike { get; init; }
        public required double BarrierLevel { get; init; }
        public required DateTime Maturity { get; init; }
        public double Notional { get; init; } = 1;
    }
}
