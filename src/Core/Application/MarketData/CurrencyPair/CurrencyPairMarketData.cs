using Domain;

namespace Application {
    public class CurrencyPairMarketData : IUnderlyingMarketData {
        private double _spot;
        private ILocalVolatilityModel? _volatility;

        public double GetCarry() {
            return 0; // drift is done on engine level. Should we fix the interface?
        }

        public double GetSpot() {
            return _spot;
        }

        public ILocalVolatilityModel GetVolatility() {
            return _volatility;
        }

        public CurrencyPairMarketData SetSpot(double spot) {
            _spot = spot;
            return this;
        }

        public CurrencyPairMarketData SetVolatility(ILocalVolatilityModel volatilityModel) {
            _volatility = volatilityModel;
            return this;    
        }

        public CurrencyPairMarketData SetVolatility(double volatility) {
            _volatility = new ConstantLocalVolatilityModel(volatility);
            return this;
        }
    }
}
