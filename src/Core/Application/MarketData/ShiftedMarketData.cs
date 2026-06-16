using Domain;

namespace Application {
    public class ShiftedMarketData : IMarketData {

        private IMarketData _marketData;
        private Dictionary<Underlying, IShiftedUnderlyingMarketData> _shifts = new();
        private Dictionary<Currency, double> _discountShifts = new();
        private Dictionary<(Underlying, Underlying), double> _correlationsShifts = new();

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

        public ShiftedMarketData ShiftCorrelation(Underlying underlying1, Underlying underlying2, double shift) {
            var key = GetCorrelationKey(underlying1, underlying2);
            _correlationsShifts[key] = shift;
            return this;
        }

        public ShiftedMarketData SetCorrelation(Underlying underlying1, Underlying underlying2, double correlation) {
            var key = GetCorrelationKey(underlying1, underlying2);
            var currentCorrelation = _marketData.GetCorrelation(underlying1, underlying2);
            _correlationsShifts[key] = correlation - currentCorrelation;
            return this;
        }

        private (Underlying, Underlying) GetCorrelationKey(Underlying u1, Underlying u2) {
            return u1.Code.CompareTo(u2.Code) < 0 ? (u1, u2) : (u2, u1);
        }

        public override int GetHashCode() {
            return HashCode.Combine(_marketData, _shifts.GetContentHashCode(), _discountShifts.GetContentHashCode(), _correlationsShifts.GetContentHashCode());
        }

        public override bool Equals(object? obj) {
            if (obj is ShiftedMarketData other) {
                return _marketData.Equals(other._marketData)
                    && _shifts.SequenceEqual(other._shifts)
                    && _discountShifts.SequenceEqual(other._discountShifts)
                    && _correlationsShifts.SequenceEqual(other._correlationsShifts);
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

        public IProcessDynamics GetShortRateDynamics(Currency currency) {
            return _marketData.GetShortRateDynamics(currency);
        }

        public IDiscounter GetDiscounter(Currency currency) {
            if (!_discountShifts.ContainsKey(currency)) {
                return _marketData.GetDiscounter(currency);
            }
            return new ShiftedDiscounter(_marketData.GetDiscounter(currency), _discountShifts[currency]);
        }

        public double GetCorrelation(Underlying underlying1, Underlying underlying2) {
            var key = GetCorrelationKey(underlying1, underlying2);
            if (_correlationsShifts.ContainsKey(key)) {
                return _marketData.GetCorrelation(underlying1, underlying2) + _correlationsShifts[key];
            }
            return _marketData.GetCorrelation(underlying1, underlying2);
        }
    }
}
