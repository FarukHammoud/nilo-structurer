using Domain;

namespace PricerServices {
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
                IDiscounter foreignDiscounter = _marketData.GetDiscounter(equity.Currency);
                double foreignDiscountFactor = foreignDiscounter.GetDiscountFactor(t1, t0);
                return foreignDiscountFactor / diffusionDiscountFactor;
            }

            if (_underlying is CurrencyPair fx) {
                IDiscounter baseDiscounter = _marketData.GetDiscounter(fx.Base);
                IDiscounter quoteDiscounter = _marketData.GetDiscounter(fx.Quote);
                return quoteDiscounter.GetDiscountFactor(t1, t0)
                            / baseDiscounter.GetDiscountFactor(t1, t0);
            }
            throw new NotImplementedException();
        }
    }
}
