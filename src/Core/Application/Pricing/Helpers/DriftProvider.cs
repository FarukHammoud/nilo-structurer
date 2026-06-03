using Domain;

namespace Application {
    public class DriftProvider : IDriftProvider {
        public double GetDrift(Underlying underlying, Currency diffusionCurrency, IMarketData marketData, DateTime t_1, DateTime t) {
            double μ = 0;
            IDiscounter discounter = marketData.GetDiscounter(diffusionCurrency);
            if (underlying is Equity equity) {
                μ = discounter.GetForwardRate(t_1, t);
                if (equity.Currency != diffusionCurrency) {
                    IDiscounter foreignDiscounter = marketData.GetDiscounter(underlying.Currency);
                    μ = foreignDiscounter.GetForwardRate(t_1, t);
                    CurrencyPair fxPair = new CurrencyPair(equity.Currency, diffusionCurrency);
                    double sigma_S = marketData.GetUnderlyingMarketData(equity).GetVolatility().getVolatility(0, 0); // todo
                    double sigma_X = marketData.GetUnderlyingMarketData(fxPair).GetVolatility().getVolatility(0, 0);
                    double rho = marketData.GetCorrelation(equity, fxPair);
                    double quantoAdjustment = - rho * sigma_S * sigma_X;
                    μ += quantoAdjustment;
                }
            } else if (underlying is CurrencyPair fxPair) {
                IDiscounter baseDiscounter = marketData.GetDiscounter(fxPair.Base);
                IDiscounter quoteDiscounter = marketData.GetDiscounter(fxPair.Quote);
                double r_base = baseDiscounter.GetForwardRate(t_1, t);
                double r_quote = quoteDiscounter.GetForwardRate(t_1, t);
                μ = r_quote - r_base;
            }
            return μ;
        }
    }
}
