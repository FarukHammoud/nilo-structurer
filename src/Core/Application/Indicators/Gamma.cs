using Domain;

namespace Application {
    public class Gamma : IIndicator {
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return GetShiftedMarketDataByUnderlying(marketData, pricingDate).Values
                .SelectMany(marketDataList => marketDataList)
                .ToList();
        }

        private Dictionary<Underlying, List<(IMarketData, DateTime)>> GetShiftedMarketDataByUnderlying(IMarketData marketData, DateTime pricingDate) {
            return marketData.Underlyings.ToDictionary(underlying => underlying, 
                underlying => new List<(IMarketData, DateTime)>() {
                    (new ShiftedMarketData(marketData)
                        .WithShift(underlying, new SpotShift(0.99)), pricingDate),
                    (marketData, pricingDate),
                    (new ShiftedMarketData(marketData)
                        .WithShift(underlying, new SpotShift(1.01)), pricingDate)
                });
        }

        public IIndicatorResult GetResult(IContract contract, IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceEstimate> resultsByShift) {   
            Dictionary<Underlying, List<(IMarketData, DateTime)>> marketDataByUnderlying = GetShiftedMarketDataByUnderlying(unshiftedMarketData, pricingDate);
            ByUnderlyingIndicatorResult result = new();
            foreach (Underlying underlying in marketDataByUnderlying.Keys) {
                IUnderlyingMarketData underlyingMarketData = unshiftedMarketData.GetUnderlyingMarketData(underlying);
                PriceEstimate valueDown = resultsByShift[marketDataByUnderlying[underlying][0]];
                PriceEstimate centralValue = resultsByShift[marketDataByUnderlying[underlying][1]];
                PriceEstimate valueUp = resultsByShift[marketDataByUnderlying[underlying][2]]; double deltaValue = (valueUp.Value - valueDown.Value) / (0.02 * underlyingMarketData.GetSpot());
                double gammaValue = (valueUp.Value - 2 * centralValue.Value + valueDown.Value) / Math.Pow(0.02 * underlyingMarketData.GetSpot(), 2);
                double gammaPrecision = Math.Sqrt(Math.Pow(valueUp.StandardError, 2) + 4 * Math.Pow(centralValue.StandardError, 2) + Math.Pow(valueDown.StandardError, 2)) / Math.Pow(0.02 * underlyingMarketData.GetSpot(), 2);
                result[underlying] = new Estimate(value : gammaValue, standardError : gammaPrecision);
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
