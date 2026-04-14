using Domain;

namespace Application {
    public class Converse : SinglePayoffNonPathDependentContract {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public override INonPathDependentPayoff Payoff =>
            new MonoUnderlyingNonPathDependentPayoff(
                spot => Notional * (spot - Strike), Underlying);
    }
}
