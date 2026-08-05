using Domain;

namespace Application {
    public class CurveDiscounter : IDiscounter {
        public required Curve Curve { get; init; }

        public IDayCountConvention DayCounter => new Actual365();

        public double GetDiscountFactor(DateTime from, DateTime to) {
            return Curve.GetValue(to) / Curve.GetValue(from);
        }
    }
}
