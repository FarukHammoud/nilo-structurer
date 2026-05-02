using Domain;

namespace Application {
    public class ShiftedMarketData : IMarketData {

        private IMarketData _marketData;
        private Dictionary<Underlying, ShiftedUnderlyingMarketData> _shifts = new();
        private Dictionary<Currency, double> _discountShifts = new();
        public ShiftedMarketData(IMarketData marketData) {
            _marketData = marketData;
        }

        public ShiftedMarketData ShiftSpot(Underlying underlying, double shift) {
            GetOrCreate(underlying).ShiftSpot(shift);
            return this;
        }

        public ShiftedMarketData ShiftDiscountRate(Currency currency, double shift) {
            _discountShifts[currency] = shift;
            return this;
        }

        public ShiftedMarketData ShiftVolatility(Underlying underlying, double shift) {
            GetOrCreate(underlying).ShiftVolatility(shift);
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

        public List<Underlying> GetUnderlyings() {
            return _marketData.GetUnderlyings();
        }

        public override int GetHashCode() {
            return HashCode.Combine(_marketData, _shifts.GetContentHashCode(), _discountShifts.GetContentHashCode());
        }

        public override bool Equals(object? obj) {
            if (obj is ShiftedMarketData other) {
                return _marketData.Equals(other._marketData)
                    && _shifts.SequenceEqual(other._shifts)
                    && _discountShifts.SequenceEqual(other._discountShifts);
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

        public double GetFxRate(Currency from, Currency to) {
            return _marketData.GetFxRate(from, to);
        }

        public IDiscounter GetDiscounter(Currency currency) {
            if (!_discountShifts.ContainsKey(currency)) {
                return _marketData.GetDiscounter(currency);
            }
            return new ShiftedDiscounter(_marketData.GetDiscounter(currency), _discountShifts[currency]);
        }
    }
}
