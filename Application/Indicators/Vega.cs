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
            return marketData.GetUnderlyings().ToDictionary(underlying => underlying,
                underlying => new List<(IMarketData, DateTime)>() {
                    (new ShiftedMarketData(marketData)
                        .ShiftVolatility(underlying, -_bump), pricingDate),
                    (new ShiftedMarketData(marketData)
                        .ShiftVolatility(underlying, +_bump), pricingDate)
                });
        }

        public IIndicatorResult GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceWithPrecision> resultsByShift) {
            Dictionary<Underlying, List<(IMarketData, DateTime)>> marketDataByUnderlying = GetShiftedMarketDataByUnderlying(unshiftedMarketData, pricingDate);
            ByUnderlyingIndicatorResult result = new();
            foreach (Underlying underlying in marketDataByUnderlying.Keys) {
                IUnderlyingMarketData underlyingMarketData = unshiftedMarketData.GetUnderlyingMarketData(underlying);
                PriceWithPrecision valueDown = resultsByShift[marketDataByUnderlying[underlying][0]];
                PriceWithPrecision valueUp = resultsByShift[marketDataByUnderlying[underlying][1]];
                double vegaValue = (valueUp.Value - valueDown.Value) / (2 * _bump);
                double vegaPrecision = (valueUp.Precision + valueDown.Precision) / 2;
                result.Result[underlying] = new ValueWithPrecision() { Value = vegaValue, Precision = vegaPrecision };
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
