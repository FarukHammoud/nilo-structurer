using Domain;

namespace Application {
    public class UnderlyingDiscounterProvider : IDiscounter {

        private Underlying _underlying;
        private Currency _diffusionCurrency;
        private IMarketData _marketData;

        public UnderlyingDiscounterProvider(Underlying underlying, Currency diffusionCurrency, IMarketData marketData) {
            _underlying = underlying;
            _diffusionCurrency = diffusionCurrency;
            _marketData = marketData;
        }

        public double GetDiscountFactor(DateTime t1, DateTime t0) {
            IDiscounter domesticDiscounter = _marketData.GetDiscounter(_diffusionCurrency);

            if (_underlying is Equity equity) {
                double diffusionDiscountFactor = domesticDiscounter.GetDiscountFactor(t1, t0);
                if (equity.Currency == _diffusionCurrency) {
                    return diffusionDiscountFactor;
                }
                IDriftProvider driftProvider = new DriftProvider();
                double drift = driftProvider.GetDrift(equity, _diffusionCurrency, _marketData, t0, t1);
                return Math.Exp(-drift * (t1 - t0).TotalYears);
            }

            if (_underlying is CurrencyPair fx) {
                // eventually merge with driftProvider logic
                IDiscounter baseDiscounter = _marketData.GetDiscounter(fx.Base);
                IDiscounter quoteDiscounter = _marketData.GetDiscounter(fx.Quote);
                return quoteDiscounter.GetDiscountFactor(t1, t0)
                            / baseDiscounter.GetDiscountFactor(t1, t0);
            }

            if (_underlying is ShortRate shortRate) {
                throw new NotImplementedException(); // should be based on shortRate realization
            }
            throw new NotImplementedException();
        }
    }
}
