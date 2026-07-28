using Domain;

namespace Application {
    public class DownAndInPayoff : KnockInPayoff {
        public DownAndInPayoff(IPayoff basePayoff, double level, Underlying underlying, DateTime startDate)
            : base(basePayoff, level, underlying, startDate) {
        }

        public override bool IsUp => false;
    }
}