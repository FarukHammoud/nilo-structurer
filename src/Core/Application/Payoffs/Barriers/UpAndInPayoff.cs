using Domain;

namespace Application{
    public class UpAndInPayoff : KnockInPayoff {
        public UpAndInPayoff(IPayoff basePayoff, double level, Underlying underlying, DateTime startDate)
            : base(basePayoff, level, underlying, startDate) {
        }

        public override bool IsUp => true;
    }
}