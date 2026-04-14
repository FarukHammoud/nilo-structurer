using Domain;

namespace Application {
    public class GeometricAsianCall : PathDependentContract {

        // needs to adapt the payoff to be geometric average instead of arithmetic average, but otherwise is the same as AsianCall
        public override IPathDependentPayoff Payoff => 
            new MonoUnderlyingPathDependentPayoff(d => Math.Max(0, d.Values.Average() - Strike), FixingDates, Underlying);
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required List<DateTime> FixingDates { get; set; } = new List<DateTime>();
    }
}
