using Domain;

namespace Application {
    public class MarketData : IMarketData {

        private Double? _riskFreeRate = null;
        private Curve _discountCurve = new();
        private List<Underlying> _underlyings = new();
        private Dictionary<Underlying, double> _spots = new();
        private Dictionary<Underlying, double> _drifts = new();
        private Dictionary<Underlying, ILocalVolatilityModel> _volatilities = new();
        private Double[,]? _correlationMatrix = null;
        public double[,] GetCorrelationMatrix(List<Underlying> underlyings) {
            return _correlationMatrix;
        }

        public double GetSpot(Underlying underlying) {
            return _spots[underlying];
        }

        public double GetDrift(Underlying underlying) {
            return _drifts[underlying];
        }

        public ILocalVolatilityModel GetVolatility(Underlying underlying) {
            return _volatilities[underlying];
        }

        public MarketData SetSpot(Underlying underlying, double spot) {
            _spots[underlying] = spot;
            return this;
        }

        public MarketData SetDrift(Underlying underlying, double drift) {
            _drifts[underlying] = drift;
            return this;
        }

        public MarketData SetVolatility(Underlying underlying, ILocalVolatilityModel volatilityModel) {
            _volatilities[underlying] = volatilityModel;
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
    }
}
