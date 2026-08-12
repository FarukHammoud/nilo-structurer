using Domain;

namespace Application {
    public class EquityMarketData : IEquityMarketData {
        private double _dividend;
        private double _repo;
        private double _spot;
        private IImpliedVolatilityModel? _volatility;

        public double GetCarry() {
            return GetDividend() + GetRepo();
        }

        public double GetDividend() {
            return _dividend;
        }

        public double GetRepo() {
            return _repo;
        }

        public double GetSpot() {
            return _spot;
        }

        public IImpliedVolatilityModel GetVolatility() {
            return _volatility;
        }

        public EquityMarketData SetDividend(double dividend) {
            _dividend = dividend;
            return this;
        }

        public EquityMarketData SetRepo(double repo) {
            _repo = repo;
            return this;
        }

        public EquityMarketData SetSpot(double spot) {
            _spot = spot;
            return this;
        }

        public EquityMarketData SetVolatility(IImpliedVolatilityModel volatilityModel) {
            _volatility = volatilityModel;
            return this;
        }

        public EquityMarketData SetVolatility(double volatility) {
            _volatility = new ConstantVolatilityModel(volatility);
            return this;
        }
    }
}
