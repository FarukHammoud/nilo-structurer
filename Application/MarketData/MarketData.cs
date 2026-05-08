using Domain;
using MathNet.Numerics.LinearAlgebra;

namespace Application {
    public class MarketData : IMarketData {
        private Dictionary<Currency, IDiscounter> _discounters = new();
        private List<Underlying> _underlyings = new();
        private OrderedDictionary<Underlying, IUnderlyingMarketData> _underlyingMarketData = new();
        private Double[,]? _correlationMatrix = null;

        public IList<Underlying> Underlyings => _underlyingMarketData.Keys.ToList();
        public IList<Currency> Currencies => _discounters.Keys.ToList();
        public double[,] GetCorrelationMatrix(IList<Underlying> underlyings) {
            if (_correlationMatrix == null) {
                int n = underlyings.Count;
                return Matrix<double>.Build.DenseIdentity(n).ToArray();
            }
            return _correlationMatrix;
        }

        public MarketData For<T>(Underlying underlying, Action<T> configure) where T : IUnderlyingMarketData {
            configure((T)GetOrCreate(underlying));
            return this;
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

        

        public IUnderlyingMarketData GetUnderlyingMarketData(Underlying underlying) {
            return _underlyingMarketData[underlying];
        }

        private IUnderlyingMarketData GetOrCreate(Underlying underlying) {
            if (!_underlyingMarketData.TryGetValue(underlying, out var marketData)) {
                if (underlying is CurrencyPair) {
                    marketData = new CurrencyPairMarketData();
                } else if (underlying is Equity) {
                    marketData = new EquityMarketData();
                }
                _underlyingMarketData[underlying] = marketData;
                _underlyings.Add(underlying);
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
