using Domain;

namespace Application {
    public class Delta : IIndicator {
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return GetShiftedMarketDataByUnderlying(marketData, pricingDate).Values
                .SelectMany(marketDataList => marketDataList)
                .ToList();
        }

        private Dictionary<Underlying, List<(IMarketData, DateTime)>> GetShiftedMarketDataByUnderlying(IMarketData marketData, DateTime pricingDate) {
            return marketData.GetUnderlyings().ToDictionary(underlying => underlying, 
                underlying => new List<(IMarketData, DateTime)>() {
                    (new ShiftedMarketData(marketData)
                        .ShiftSpot(underlying, 0.99), pricingDate),
                    (new ShiftedMarketData(marketData)
                        .ShiftSpot(underlying, 1.01), pricingDate)
                });
        }

        public IIndicatorResult GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), ValueWithPrecision> resultsByShift) {
            Dictionary<Underlying, List<(IMarketData, DateTime)>> marketDataByUnderlying = GetShiftedMarketDataByUnderlying(unshiftedMarketData, pricingDate);
            ByUnderlyingIndicatorResult result = new();
            foreach (Underlying underlying in marketDataByUnderlying.Keys) {
                IUnderlyingMarketData underlyingMarketData = unshiftedMarketData.GetUnderlyingMarketData(underlying);
                ValueWithPrecision valueDown = resultsByShift[marketDataByUnderlying[underlying][0]];
                ValueWithPrecision valueUp = resultsByShift[marketDataByUnderlying[underlying][1]];
                double deltaValue = (valueUp.Value - valueDown.Value) / (0.02 * underlyingMarketData.GetSpot());
                double deltaPrecision = (valueUp.Precision + valueDown.Precision) / 2;
                result.Result[underlying] = new ValueWithPrecision() { Value = deltaValue, Precision = deltaPrecision };
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
