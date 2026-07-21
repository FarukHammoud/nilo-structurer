using Domain;

namespace Application {
    public class ShortRateDiscounter : IDiscounter {
        private readonly IDiscounter _internalDiscounter;
        public ShortRateDiscounter(SimulatedPath shortRatePath, IList<DateTime> timeDiscretization) {
            Curve curve = new Curve();
            IList<DateTime> dates = timeDiscretization;
            double integral = 0;
            for (int k = 0; k < shortRatePath.Values.Count() - 1; k++) {
                double dt = (dates[k + 1] - dates[k]).TotalYears;
                integral += shortRatePath.Values[k] * dt;
                curve.setNode(dates[k], Math.Exp(-integral));
            }
            _internalDiscounter = new CurveDiscounter {
                Curve = curve
            };
        }

        public double GetDiscountFactor(DateTime date, DateTime today) {
            return _internalDiscounter.GetDiscountFactor(date, today);
        }
    }
}
