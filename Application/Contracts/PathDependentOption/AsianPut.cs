using Domain;

namespace Application {
    public class AsianPut : PathDependentContract {

        public override IPathDependentPayoff Payoff => 
            new MonoUnderlyingPathDependentPayoff(d => Math.Max(0, Strike - d.Values.Average()), FixingDates, Underlying);
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required List<DateTime> FixingDates { get; set; } = new List<DateTime>();
    }
}
