using Domain;

namespace Application {
    public class CurveDiscounter : IDiscounter {
        public required Curve Curve { get; init; }
        public double GetDiscountFactor(DateTime date, DateTime today) {
            return Curve.GetValue(date) / Curve.GetValue(today);
        }
    }
}
