using Domain;

namespace Application {
    public class MarketData : IMarketData {
        private Dictionary<Currency, IDiscounter> _discounters = new();
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
            GetOrCreate(underlying).SetSpot(spot);
            return this;
        }

        public MarketData SetVolatility(Underlying underlying, ILocalVolatilityModel volatilityModel) {
            GetOrCreate(underlying).SetVolatility(volatilityModel);
            return this;
        }
        public MarketData SetVolatility(Underlying underlying, double volatility) {
            return SetVolatility(underlying, new ConstantLocalVolatilityModel(volatility));
        }

        public MarketData SetCorrelationMatrix(double[,] correlationMatrix) {
            _correlationMatrix = correlationMatrix;
            return this;
        }

        public MarketData SetRiskFreeRate(Currency currency, double riskFreeRate) {
            _discounters[currency] = new FixedRateDiscounter() { Rate = riskFreeRate };
            return this;
        }

        public MarketData SetDiscountCurve(Currency currency, Curve discountCurve) {
            _discounters[currency] = new CurveDiscounter() { Curve = discountCurve };
            return this;
        }

        public IDiscounter GetDiscounter(Currency currency) {
            return _discounters[currency];
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

        private UnderlyingMarketData GetOrCreate(Underlying underlying) {
            if (!_underlyingMarketData.TryGetValue(underlying, out var marketData)) {
                marketData = new UnderlyingMarketData();
                _underlyingMarketData[underlying] = marketData;
            }
            return marketData;
        }

        public double GetFxRate(Currency from, Currency to) {
            if (from.Equals(to)) {
                return 1.0;
            }

            // Direct pair
            CurrencyPair direct = new(from, to);
            if (_underlyingMarketData.ContainsKey(direct)) {
                return _underlyingMarketData[direct].GetSpot();
            }

            // Inverse pair
            CurrencyPair inverse = new(to, from);
            if (_underlyingMarketData.ContainsKey(inverse)) {
                return 1.0 / _underlyingMarketData[inverse].GetSpot();
            }

            // Triangulation through domestic currency
            // e.g. EURGBP = EURUSD / GBPUSD
            foreach (var spot in _underlyingMarketData) {
                if (spot.Key is CurrencyPair pair) {
                    if (pair.Base.Equals(from)) {
                        double leg1 = spot.Value.GetSpot(); // from → pair.Quote
                        double leg2 = GetFxRate(pair.Quote, to);
                        return leg1 * leg2;
                    }
                }
            }

            throw new InvalidOperationException(
                $"No FX rate found for {from.Code}/{to.Code}");
        }
    }
}
