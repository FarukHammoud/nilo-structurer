using Domain;

namespace Application {
    public class AsianCall : SinglePayoffPathDependentContract {

        public override IPathDependentPayoff Payoff => 
            new MonoUnderlyingPathDependentPayoff() {
                PayoffMap = d => Math.Max(0, d.Values.Average() - Strike),
                ObservationDates = FixingDates,
                Underlying = Underlying,
                MonitoringFrequency = MonitoringFrequency.None
            };
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required List<DateTime> FixingDates { get; set; } = new List<DateTime>();
    }
}
