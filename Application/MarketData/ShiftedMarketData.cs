using Domain;

namespace Application {
    public class ShiftedMarketData : IMarketData {

        private IMarketData _marketData;
        private Dictionary<Underlying, double> _spotShifts = new Dictionary<Underlying, double>();
        private double? _discountRateShift;
        public ShiftedMarketData(IMarketData marketData) {
            _marketData = marketData;
        }

        public ShiftedMarketData ShiftSpot(Underlying underlying, double shift) {
            _spotShifts[underlying] = shift;
            return this;
        }

        public ShiftedMarketData ShiftDiscountRate(double shift) {
            _discountRateShift = shift;
            return this;
        }

        public double[,] GetCorrelationMatrix(List<Underlying> underlyings) {
            return _marketData.GetCorrelationMatrix(underlyings);
        }

        public double GetDiscountFactor(DateTime date, DateTime today) {
            double discountFactor = _marketData.GetDiscountFactor(date, today);
            if (_discountRateShift.HasValue) {
                discountFactor *= Math.Exp(-_discountRateShift.Value * (date - today).TotalDays / 365.0);
            }
            return discountFactor;
        }

        public double GetDrift(Underlying underlying) {
            if (_discountRateShift.HasValue) {
                return _marketData.GetDrift(underlying) + _discountRateShift.Value;
            } 
            return _marketData.GetDrift(underlying);
        }

        public double GetSpot(Underlying underlying) {
            double spot = _marketData.GetSpot(underlying);
            if (_spotShifts.ContainsKey(underlying)) {
                spot *= _spotShifts[underlying];
            }
            return spot;
        }

        public ILocalVolatilityModel GetVolatility(Underlying underlying) {
            return _marketData.GetVolatility(underlying);
        }

        public List<Underlying> GetUnderlyings() {
            return _marketData.GetUnderlyings();
        }

        public override int GetHashCode() {
            return _marketData.GetHashCode() + _spotShifts.GetContentHashCode();
        }

        public override bool Equals(object? obj) {
            if (obj is ShiftedMarketData other) {
                return _marketData.Equals(other._marketData) && _spotShifts.SequenceEqual(other._spotShifts) && _discountRateShift == other._discountRateShift;
            }
            return false;
        }
    }
}
