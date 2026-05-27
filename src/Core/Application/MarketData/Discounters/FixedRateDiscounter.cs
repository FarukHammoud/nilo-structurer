using Domain;

namespace Application {
    public class FixedRateDiscounter : IDiscounter {
        public double Rate { get; init; }
        public double GetDiscountFactor(DateTime date, DateTime today) {
            return Math.Exp(-Rate * (date - today).TotalYears);
        }
    }
}
