using Domain;

namespace Application{
    public class UpAndInPayoff : KnockInPayoff {
        public UpAndInPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying)
            : base(basePayoff, level, underlying) {
        }

        public override Func<Dictionary<DateTime, double>, bool> IsTouched => prices => prices.Values.Any(price => price >= Level);
    }
}