using Domain;

namespace Application {
    public class AsianCall : SinglePayoffPathDependentContract {

        public override IPathDependentPayoff Payoff => 
            new MonoUnderlyingPathDependentPayoff(d => Math.Max(0, d.Values.Average() - Strike), FixingDates, Underlying);
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required List<DateTime> FixingDates { get; set; } = new List<DateTime>();
    }
}
