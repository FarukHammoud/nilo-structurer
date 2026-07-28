using Domain;

namespace Application{
    public class UpAndOutPayoff : KnockOutPayoff {
        public UpAndOutPayoff(IPayoff basePayoff, double level, Underlying underlying, DateTime startDate)
            : base(basePayoff, level, underlying, startDate) {
        }
        
        public override bool IsUp => true;
    }
}