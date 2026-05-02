using Domain;

namespace Application {
    public class ShiftedDiscounter : IDiscounter {
        private readonly IDiscounter _base;
        private readonly double _rateShift;

        public ShiftedDiscounter(IDiscounter base_, double rateShift) {
            _base = base_;
            _rateShift = rateShift;
        }

        public double GetDiscountFactor(DateTime date, DateTime today) {
            return _base.GetDiscountFactor(date, today) * Math.Exp(-_rateShift * ((date-today).TotalDays / 365.0));
        }

    }
}
