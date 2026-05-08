using Domain;

namespace Application {
    public class ShiftedMarketData : IMarketData {

        private IMarketData _marketData;
        private Dictionary<Underlying, IShiftedUnderlyingMarketData> _shifts = new();
        private Dictionary<Currency, double> _discountShifts = new();

        public IList<Underlying> Underlyings => _marketData.Underlyings;

        public IList<Currency> Currencies => _marketData.Currencies;

        public ShiftedMarketData(IMarketData marketData) {
            _marketData = marketData;
        }

        public ShiftedMarketData WithShift(Underlying underlying, UnderlyingShift shift) {
            GetOrCreate(underlying).WithShift(shift);
            return this;
        }

        public IShiftedUnderlyingMarketData GetOrCreate(Underlying underlying) {
            if (!_shifts.TryGetValue(underlying, out var shifted)) {
                if (underlying is Equity equity) {
                    shifted = new ShiftedEquityMarketData(_marketData.GetUnderlyingMarketData(equity));
                } else if (underlying is CurrencyPair currencyPair){
                    shifted = new ShiftedCurrencyPairMarketData(_marketData.GetUnderlyingMarketData(currencyPair));
                }
                _shifts[underlying] = shifted;
            }
            return shifted;
        }

        public ShiftedMarketData ShiftDiscountRate(Currency currency, double shift) {
            _discountShifts[currency] = shift;
            return this;
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
            if (_shifts.Keys.Contains(underlying)) {
                return _shifts[underlying];
            }
            return _marketData.GetUnderlyingMarketData(underlying);
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

        public double[,] GetCorrelationMatrix(IList<Underlying> underlyings) {
            return _marketData.GetCorrelationMatrix(underlyings);
        }
    }
}
