using Domain;

namespace Application {
    public class DownAndOutPayoff : KnockOutPayoff {
        public DownAndOutPayoff(IPayoff basePayoff, double level, Underlying underlying, DateTime startDate)
            : base(basePayoff, level, underlying, startDate) {
        }

        public override bool IsUp => false;
    }
}