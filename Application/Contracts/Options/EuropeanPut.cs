using Domain;

namespace Application {
    public class EuropeanPut : Put {
        public override INonPathDependentPayoff Payoff => 
            new MonoUnderlyingNonPathDependentPayoff(
                spot => Notional * Math.Max(0, Strike - spot), Underlying);
    }
}
