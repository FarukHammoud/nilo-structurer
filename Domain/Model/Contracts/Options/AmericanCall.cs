namespace Domain {
    public class AmericanCall : Call {
        public override INonPathDependentPayoff Payoff => new MonoUnderlyingNonPathDependentPayoff(
            spot => Notional * Math.Max(0, spot - Strike), Underlying);
    }
}
