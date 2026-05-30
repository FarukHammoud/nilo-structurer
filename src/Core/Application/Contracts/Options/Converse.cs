using Domain;

namespace Application {
    public class Converse : SinglePayoffPathIndependentContract {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public override IPathIndependentPayoff Payoff =>
            new MonoUnderlyingPathIndependentPayoff(
                spot => Notional * (spot - Strike), Underlying, Currency);
    }
}
