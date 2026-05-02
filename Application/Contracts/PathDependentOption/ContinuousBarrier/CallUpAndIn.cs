using Domain;

namespace Application {
    public class CallUpAndIn : IPathDependentContract {
        public IEnumerable<Tuple<DateTime, IPathDependentPayoff>> Payoffs => [Tuple.Create(Maturity, 
            (IPathDependentPayoff) new UpAndInPayoff(
                new MonoUnderlyingPathDependentPayoff() {
                    PayoffMap = d => Math.Max(0, d.Values.Last() - Strike),
                    ObservationDates = new List<DateTime>() { Maturity },
                    Underlying = Underlying,
                    MonitoringFrequency = MonitoringFrequency.Continuous,
                    Currency = Currency},
                    BarrierLevel, Underlying))];
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Strike { get; set; }
        public required double BarrierLevel { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; }
    }
}
