using Domain;

namespace Application {
    public class DownAndOutPayoff : KnockOutPayoff {
        public DownAndOutPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying)
            : base(basePayoff, level, underlying) {
        }

        public override Func<SimulatedPath, bool> IsTouched => prices => prices.Values.Any(price => price <= Level);
    }
}