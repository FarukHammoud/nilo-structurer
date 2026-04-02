namespace Domain {
    public class MarketData : IMarketData {

        private double _riskFreeRate;
        private List<Underlying> underlyings = new();
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

        public double GetDiscountFactor(DateTime date, DateTime today) {
            return Math.Exp(-_riskFreeRate * (date - today).TotalDays / 365.0);
        }
    }
}
