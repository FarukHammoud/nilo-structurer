using Domain;

namespace Application {
    public class ShortRateDiscounter : IDiscounter {
        private readonly IDiscounter _internalDiscounter;
        public IDayCountConvention DayCounter { get; init; } = new Actual365();
        public ShortRateDiscounter(SimulatedPath shortRatePath, IList<DateTime> timeDiscretization) {
            Curve curve = new Curve();
            IList<DateTime> dates = timeDiscretization;
            double integral = 0;
            for (int k = 0; k < shortRatePath.Values.Count() - 1; k++) {
                double dt = DayCounter.YearFraction(dates[k], dates[k + 1]);
                integral += shortRatePath.Values[k] * dt;
                curve.setNode(dates[k], Math.Exp(-integral));
            }
            _internalDiscounter = new CurveDiscounter {
                Curve = curve
            };
        }

        public double GetDiscountFactor(DateTime from, DateTime to) {
            return _internalDiscounter.GetDiscountFactor(from, to);
        }
    }
}
