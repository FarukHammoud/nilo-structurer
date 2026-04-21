using Domain;

namespace Application {
    public class GeometricAsianPut : SinglePayoffPathDependentContract {

        // needs to adapt the payoff to be geometric average instead of arithmetic average, but otherwise is the same as AsianCall
        public override IPathDependentPayoff Payoff => 
            new MonoUnderlyingPathDependentPayoff() {
                PayoffMap = d => Math.Max(0, Strike - Math.Exp(d.Values.Select(a => Math.Log(a)).Average())),
                ObservationDates = FixingDates,
                Underlying = Underlying,
                MonitoringFrequency = MonitoringFrequency.None
            };
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required List<DateTime> FixingDates { get; set; } = new List<DateTime>();
    }
}
