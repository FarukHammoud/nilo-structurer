using Domain;

namespace Application {
    public class UnderlyingDiscounterProvider : IDiscounter {

        private Underlying _underlying;
        private Currency _diffusionCurrency;
        private IMarketData _marketData;

        public IDayCountConvention DayCounter => new ActualActualIsda();

        public UnderlyingDiscounterProvider(Underlying underlying, Currency diffusionCurrency, IMarketData marketData) {
            _underlying = underlying;
            _diffusionCurrency = diffusionCurrency;
            _marketData = marketData;
        }

        public double GetDiscountFactor(DateTime t0, DateTime t1) {
            IDiscounter domesticDiscounter = _marketData.GetDiscounter(_diffusionCurrency);

            if (_underlying is Equity equity) {
                double diffusionDiscountFactor = domesticDiscounter.GetDiscountFactor(t0, t1);
                if (equity.Currency == _diffusionCurrency) {
                    return diffusionDiscountFactor;
                }
                IDriftProvider driftProvider = new DriftProvider();
                double drift = driftProvider.GetDrift(equity, _diffusionCurrency, _marketData, t0, t1);
                return Math.Exp(-drift * DayCounter.YearFraction(t0, t1));
            }

            if (_underlying is CurrencyPair fx) {
                // eventually merge with driftProvider logic
                IDiscounter baseDiscounter = _marketData.GetDiscounter(fx.Base);
                IDiscounter quoteDiscounter = _marketData.GetDiscounter(fx.Quote);
                return quoteDiscounter.GetDiscountFactor(t0, t1)
                            / baseDiscounter.GetDiscountFactor(t0, t1);
            }

            if (_underlying is ShortRate shortRate) {
                throw new NotImplementedException(); // should be based on shortRate realization
            }
            throw new NotImplementedException();
        }
    }
}
