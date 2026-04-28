using Domain;

namespace Application {
    public class Gamma : IIndicator {
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
                    (marketData, pricingDate),
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
                ValueWithPrecision centralValue = resultsByShift[marketDataByUnderlying[underlying][1]];
                ValueWithPrecision valueUp = resultsByShift[marketDataByUnderlying[underlying][2]]; double deltaValue = (valueUp.Value - valueDown.Value) / (0.02 * underlyingMarketData.GetSpot());
                double gammaValue = (valueUp.Value - 2 * centralValue.Value + valueDown.Value) / Math.Pow(0.02 * underlyingMarketData.GetSpot(), 2);
                double gammaPrecision = Math.Sqrt(Math.Pow(valueUp.Precision, 2) + 4 * Math.Pow(centralValue.Precision, 2) + Math.Pow(valueDown.Precision, 2)) / Math.Pow(0.02 * underlyingMarketData.GetSpot(), 2);
                result.Result[underlying] = new ValueWithPrecision() { Value = gammaValue, Precision = gammaPrecision };
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
