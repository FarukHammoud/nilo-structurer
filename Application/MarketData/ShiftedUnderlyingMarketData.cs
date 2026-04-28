using Domain;

namespace Application {
    public class ShiftedUnderlyingMarketData : IUnderlyingMarketData {

        private IUnderlyingMarketData _base;
        private double _spotShift;
        private double? _volatilityShift;
        private double _repoShift;
        private double _dividendShift;

        public ShiftedUnderlyingMarketData(
            IUnderlyingMarketData base_,
            double spotShift = 1.0,       // multiplicatif
            double? volatilityShift = null,
            double repoShift = 0.0,       // additif
            double dividendShift = 0.0) { // additif
            _base = base_;
            _spotShift = spotShift;
            _volatilityShift = volatilityShift;
            _repoShift = repoShift;
            _dividendShift = dividendShift;
        }

        public double GetSpot() => _base.GetSpot() * _spotShift;
        public double GetRepo() => _base.GetRepo() + _repoShift;
        public double GetDividend() => _base.GetDividend() + _dividendShift;
        public ILocalVolatilityModel GetVolatility() => _volatilityShift.HasValue
            ? new ShiftedVolatilityModel(_base.GetVolatility(), _volatilityShift.Value)
            : _base.GetVolatility();

        public ShiftedUnderlyingMarketData ShiftSpot(double shift) {
            _spotShift = shift;
            return this;
        }

        public ShiftedUnderlyingMarketData ShiftRepo(double shift) {
            _repoShift = shift;
            return this;
        }

        public ShiftedUnderlyingMarketData ShiftDividend(double shift) {
            _dividendShift = shift;
            return this;
        }

        public ShiftedUnderlyingMarketData ShiftVolatility(double shift) {
            _volatilityShift = shift;
            return this;
        }
    }
}
