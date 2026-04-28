using Domain;

namespace Application {
    public class ShiftedMarketData : IMarketData {

        private IMarketData _marketData;
        private Dictionary<Underlying, ShiftedUnderlyingMarketData> _shifts = new();
        private double? _discountRateShift;
        private double? _volatilityShift;
        public ShiftedMarketData(IMarketData marketData) {
            _marketData = marketData;
        }

        public ShiftedMarketData ShiftSpot(Underlying underlying, double shift) {
            GetOrCreate(underlying).ShiftSpot(shift);
            return this;
        }

        public ShiftedMarketData ShiftDiscountRate(double shift) {
            _discountRateShift = shift;
            return this;
        }

        public ShiftedMarketData ShiftVolatility(double shift) {
            _volatilityShift = shift;
            return this;
        }

        public ShiftedMarketData ShiftRepo(Underlying underlying, double shift) {
            GetOrCreate(underlying).ShiftRepo(shift);
            return this;
        }

        public ShiftedMarketData ShiftDividend(Underlying underlying, double shift) {
            GetOrCreate(underlying).ShiftDividend(shift);
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

        public List<Underlying> GetUnderlyings() {
            return _marketData.GetUnderlyings();
        }

        public override int GetHashCode() {
            return HashCode.Combine(_marketData, _shifts.GetContentHashCode(), _discountRateShift, _volatilityShift);
        }

        public override bool Equals(object? obj) {
            if (obj is ShiftedMarketData other) {
                return _marketData.Equals(other._marketData)
                    && _shifts.SequenceEqual(other._shifts)
                    && _discountRateShift == other._discountRateShift
                    && _volatilityShift == other._volatilityShift;
            }
            return false;
        }

        public IUnderlyingMarketData GetUnderlyingMarketData(Underlying underlying) {
            if (_shifts.TryGetValue(underlying, out var shifted)) {
                return shifted;
            }
            return _marketData.GetUnderlyingMarketData(underlying);
        }

        private ShiftedUnderlyingMarketData GetOrCreate(Underlying underlying) {
            if (!_shifts.TryGetValue(underlying, out var shifted)) {
                shifted = new ShiftedUnderlyingMarketData(_marketData.GetUnderlyingMarketData(underlying));
                _shifts[underlying] = shifted;
            }
            return shifted;
        }

    }
}
