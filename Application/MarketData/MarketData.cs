using Domain;

namespace Application {
    public class MarketData : IMarketData {

        private Double? _riskFreeRate = null;
        private Curve _discountCurve = new();
        private List<Underlying> _underlyings = new();
        private Dictionary<Underlying, UnderlyingMarketData> _underlyingMarketData = new();
        private Double[,]? _correlationMatrix = null;
        public double[,] GetCorrelationMatrix(List<Underlying> underlyings) {
            return _correlationMatrix;
        }

        public double GetSpot(Underlying underlying) {
            return _underlyingMarketData[underlying].GetSpot();
        }

        public ILocalVolatilityModel GetVolatility(Underlying underlying) {
            return _underlyingMarketData[underlying].GetVolatility();
        }

        public MarketData SetSpot(Underlying underlying, double spot) {
            _underlyingMarketData[underlying].SetSpot(spot);
            return this;
        }

        public MarketData SetVolatility(Underlying underlying, ILocalVolatilityModel volatilityModel) {
            _underlyingMarketData[underlying].SetVolatility(volatilityModel);
            return this;
        }
        public MarketData SetVolatility(Underlying underlying, double volatility) {
            return SetVolatility(underlying, new ConstantLocalVolatilityModel(volatility));
        }

        public MarketData SetCorrelationMatrix(double[,] correlationMatrix) {
            _correlationMatrix = correlationMatrix;
            return this;
        }

        public MarketData SetRiskFreeRate(double riskFreeRate) {
            _riskFreeRate = riskFreeRate;
            return this;
        }

        public MarketData SetDiscountCurve(Curve discountCurve) {
            _discountCurve = discountCurve;
            return this;
        }

        public double GetDiscountFactor(DateTime date, DateTime today) {
            if (_riskFreeRate.HasValue) {
                return Math.Exp(-_riskFreeRate.Value * (date - today).TotalDays / 365.0);
            }
            return _discountCurve.GetValue(date) / _discountCurve.GetValue(today);          
        }

        public MarketData SetUnderlyings(List<Underlying> underlyings) {
            _underlyings = underlyings;
            return this;
        }

        public List<Underlying> GetUnderlyings() {
            return _underlyings;
        }

        public IUnderlyingMarketData GetUnderlyingMarketData(Underlying underlying) {
            return _underlyingMarketData[underlying];
        }
    }
}
