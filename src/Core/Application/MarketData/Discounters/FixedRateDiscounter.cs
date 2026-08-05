using Domain;

namespace Application {
    public class FixedRateDiscounter : IDiscounter {
        public double Rate { get; init; }
        public IDayCountConvention DayCounter { get; init; } = new Actual365();

        public double GetDiscountFactor(DateTime from, DateTime to) {
            return Math.Exp(-Rate * DayCounter.YearFraction(from, to));
        }
    }
}
