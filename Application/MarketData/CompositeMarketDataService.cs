using Domain;

namespace Application {
    public class CompositeMarketDataService {

        private readonly ISpotProvider _spotProvider;
        private readonly IVolatilityProvider _volatilityProvider;
        private readonly IRateCurveProvider _rateCurveProvider;

        public CompositeMarketDataService(
            ISpotProvider spotProvider, 
            IVolatilityProvider volatilityProvider, 
            IRateCurveProvider rateCurveProvider) {
            _spotProvider = spotProvider;
            _volatilityProvider = volatilityProvider;
            _rateCurveProvider = rateCurveProvider;
        }

        public async Task<IMarketData> GetMarketData(
            DateOnly date,
            IEnumerable<Underlying> underlyings,
            IEnumerable<Currency> currencies,
            CancellationToken ct = default) {

            MarketData marketData = new MarketData();
            var spots = await _spotProvider.GetSpotsAsync(underlyings, ct);
            var volatilities = await _volatilityProvider.GetVolatilitiesAsync(underlyings, ct);
            foreach (Underlying underlying in underlyings) {
                if (spots.TryGetValue(underlying, out var spot)) {
                    marketData.For<EquityMarketData>(underlying, md => md.SetSpot(spot));
                }
                if (volatilities.TryGetValue(underlying, out var volatility)) {
                    marketData.For<EquityMarketData>(underlying, md => md.SetVolatility(volatility));
                }
            }
            var rateCurves = await _rateCurveProvider.GetRateCurveAsync(currencies, ct);
            foreach (Currency currency in currencies) {
                if (rateCurves.TryGetValue(currency, out var rateCurve)) {
                    marketData.SetDiscountCurve(currency, rateCurve);
                }
            } 
            return marketData;
        }
    }
}
