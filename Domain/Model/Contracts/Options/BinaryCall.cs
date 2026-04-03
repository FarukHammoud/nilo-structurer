namespace Domain {
    public class BinaryCall : VanillaContract {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => spot > Strike ? Notional : 0, Underlying);
    }
}
