using Domain;

namespace Application {
    public abstract class ShiftedUnderlyingMarketData : IShiftedUnderlyingMarketData {
        protected readonly IUnderlyingMarketData _inner;
        protected IList<UnderlyingShift> _shifts = new List<UnderlyingShift>();

        protected ShiftedUnderlyingMarketData(IUnderlyingMarketData inner) {
            _inner = inner;
        }

        public double GetSpot() {
            double multiplier = _shifts.OfType<SpotShift>().Product(shift => shift.Multiplier);
            return _inner.GetSpot() * multiplier;
        }

        public double GetCarry() {
            double bump = _shifts.OfType<CarryShift>().Sum(shift => shift.Bump);
            return _inner.GetCarry() + bump;
        }

        public ILocalVolatilityModel GetVolatility() {
            double bump = _shifts.OfType<VolatilityShift>().Sum(shift => shift.Bump);
            if (bump == 0) {
                return _inner.GetVolatility();
            }
            return new ShiftedVolatilityModel(_inner.GetVolatility(), bump);
        }

        public IUnderlyingMarketData WithShift(UnderlyingShift shift) {
            _shifts.Add(shift);
            return this;
        }

        public override bool Equals(object? obj) {
            return obj is ShiftedUnderlyingMarketData data &&
                   EqualityComparer<IUnderlyingMarketData>.Default.Equals(_inner, data._inner) &&
                   _shifts.SequenceEqual(data._shifts);
        }

        public override int GetHashCode() {
            var hash = new HashCode();
            hash.Add(_inner);
            foreach (var shift in _shifts) {
                hash.Add(shift);
            }
            return hash.ToHashCode();
        }
    }
    
}
