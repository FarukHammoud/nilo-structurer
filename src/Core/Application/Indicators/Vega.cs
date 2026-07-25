using Domain;

namespace Application {
    public class Vega : IIndicator {

        private readonly double _bump;

        public Vega(double bump = 0.01) {
            _bump = bump;
        }

        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return GetShiftedMarketDataByUnderlying(marketData, pricingDate).Values
                .SelectMany(marketDataList => marketDataList)
                .ToList();
        }

        private Dictionary<Underlying, List<(IMarketData, DateTime)>> GetShiftedMarketDataByUnderlying(IMarketData marketData, DateTime pricingDate) {
            return marketData.Underlyings.ToDictionary(underlying => underlying,
                underlying => new List<(IMarketData, DateTime)>() {
                    (new ShiftedMarketData(marketData)
                        .WithShift(underlying, new VolatilityShift(-_bump)), pricingDate),
                    (new ShiftedMarketData(marketData)
                        .WithShift(underlying, new VolatilityShift(+_bump)), pricingDate)
                });
        }

        public IIndicatorResult GetResult(IContract contract, IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceEstimate> resultsByShift) {
            Dictionary<Underlying, List<(IMarketData, DateTime)>> marketDataByUnderlying = GetShiftedMarketDataByUnderlying(unshiftedMarketData, pricingDate);
            ByUnderlyingIndicatorResult result = new();
            foreach (Underlying underlying in marketDataByUnderlying.Keys) {
                IUnderlyingMarketData underlyingMarketData = unshiftedMarketData.GetUnderlyingMarketData(underlying);
                PriceEstimate valueDown = resultsByShift[marketDataByUnderlying[underlying][0]];
                PriceEstimate valueUp = resultsByShift[marketDataByUnderlying[underlying][1]];
                double vegaValue = (valueUp.Value - valueDown.Value) / (2 * _bump);
                double vegaPrecision = (valueUp.StandardError + valueDown.StandardError) / 2;
                result[underlying] = new Estimate(value: vegaValue, standardError: vegaPrecision);
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
