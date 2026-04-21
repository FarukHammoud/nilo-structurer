using Domain;
using System.Diagnostics;

namespace Application {
    public class GeometricAsianCall : SinglePayoffPathDependentContract {
        public override IPathDependentPayoff Payoff => 
            new MonoUnderlyingPathDependentPayoff() {
                PayoffMap = d => Math.Max(0, Math.Exp(d.Values.Select(a => Math.Log(a)).Average()) - Strike),
                ObservationDates = FixingDates,
                Underlying = Underlying,
                MonitoringFrequency = MonitoringFrequency.None
            };
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required List<DateTime> FixingDates { get; set; } = new List<DateTime>();
    }
}
