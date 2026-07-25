using Domain;

namespace Application {
    public class Delta : IIndicator {
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
                PriceEstimate valueUp = resultsByShift[marketDataByUnderlying[underlying][1]];
                double deltaValue = (valueUp.Value - valueDown.Value) / (0.02 * underlyingMarketData.GetSpot());
                double deltaPrecision = (valueUp.StandardError + valueDown.StandardError) / 2;
                result[underlying] = new Estimate(value : deltaValue, standardError: deltaPrecision);
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
