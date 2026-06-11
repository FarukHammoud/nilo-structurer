using Domain;

namespace Application {
    public class MarketData : IMarketData {
        private OrderedDictionary<Currency, IDiscounter> _discounters = new();
        private OrderedDictionary<Currency, IProcessDynamics> _shortRateDynamics = new();
        private OrderedDictionary<Underlying, IUnderlyingMarketData> _underlyingMarketData = new();
        private OrderedDictionary<(Underlying, Underlying), double> _correlations = new();

        public IList<Underlying> Underlyings => _underlyingMarketData.Keys.ToList();
        public IList<Currency> Currencies => _discounters.Keys.ToList();
        
        public MarketData For<T>(Underlying underlying, Action<T> configure) where T : IUnderlyingMarketData {
            configure((T)GetOrCreate(underlying));
            return this;
        }

        public MarketData SetCorrelation(Underlying u1, Underlying u2, double correlation) {
            var key = GetCorrelationKey(u1, u2);
            _correlations[key] = correlation;
            return this;
        }

        public double GetCorrelation(Underlying u1, Underlying u2) {
            if(u1 == u2) {
                return 1.0;
            }
            var key = GetCorrelationKey(u1, u2);
            if (!_correlations.TryGetValue(key, out var correlation)) {
                correlation = 0.0;
            }
            return correlation;
        }

        private (Underlying, Underlying) GetCorrelationKey(Underlying u1, Underlying u2) {
            return u1.Code.CompareTo(u2.Code) < 0 ? (u1, u2) : (u2, u1);
        }

        public MarketData SetRiskFreeRate(Currency currency, double riskFreeRate) {
            _discounters[currency] = new FixedRateDiscounter() { Rate = riskFreeRate };
            return this;
        }

        public MarketData SetDiscountCurve(Currency currency, Curve discountCurve) {
            _discounters[currency] = new CurveDiscounter() { Curve = discountCurve };
            return this;
        }

        public MarketData SetShortRateDynamics(Currency currency, IProcessDynamics dynamics) {
            _underlyingMarketData.Add(new ShortRate(currency), new ProcessDynamicsMarketData(dynamics));
            _shortRateDynamics[currency] = dynamics;
            return this;
        }

        public IDiscounter GetDiscounter(Currency currency) {
            return _discounters[currency];
        }

        public IProcessDynamics GetShortRateDynamics(Currency currency) {
            return _shortRateDynamics[currency];
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
                } else {
                    throw new InvalidOperationException($"Unsupported underlying type: {underlying.GetType().Name}");
                }
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
