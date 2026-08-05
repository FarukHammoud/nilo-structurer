using Domain;

namespace Application {
    public class ShiftedDiscounter : IDiscounter {
        private readonly IDiscounter _base;
        private readonly double _rateShift;
        public IDayCountConvention DayCounter => _base.DayCounter;

        public ShiftedDiscounter(IDiscounter base_, double rateShift) {
            _base = base_;
            _rateShift = rateShift;
        }

        public double GetDiscountFactor(DateTime from, DateTime to) {
            return _base.GetDiscountFactor(from, to) * Math.Exp(-_rateShift * DayCounter.YearFraction(from, to));
        }

    }
}
