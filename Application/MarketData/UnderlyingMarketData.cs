using Domain;

namespace Application {
    public class UnderlyingMarketData : IUnderlyingMarketData {
        private double _dividend;
        private double _repo;
        private double _spot;
        private ILocalVolatilityModel? _volatility;
        public double GetDividend() {
            return _dividend;
        }

        public double GetRepo() {
            return _repo;
        }

        public double GetSpot() {
            return _spot;
        }

        public ILocalVolatilityModel GetVolatility() {
            return _volatility;
        }

        public void SetDividend(double dividend) {
            _dividend = dividend;
        }

        public void SetRepo(double repo) {
            _repo = repo;
        }

        public void SetSpot(double spot) {
            _spot = spot;
        }

        public void SetVolatility(ILocalVolatilityModel volatilityModel) {
            _volatility = volatilityModel;
        }
    }
}
