using Domain;

namespace Application {
    public class CallDownAndIn : IPathDependentContract {
        public IEnumerable<IPathDependentPayoff> PathDependentPayoffs => [
            new DownAndInPayoff(
                new MonoUnderlyingPathDependentPayoff() {
                    PayoffMap = d => Math.Max(0, d.Values.Last() - Strike),
                    ObservationDates = [Maturity],
                    Underlying = Underlying,
                    MonitoringFrequency = MonitoringFrequency.Continuous,
                    Currency = Currency,
                    Maturity = Maturity,
                    PaymentDate = Maturity
                }, BarrierLevel, Underlying)];
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Strike { get; set; }
        public required double BarrierLevel { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; }
    }
}
