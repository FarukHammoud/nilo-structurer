using Domain;

namespace Application {
    public class Delta : IIndicator {
        public IList<IMarketData> GetShiftedMarketData(IMarketData marketData) {
            return GetShiftedMarketDataByUnderlying(marketData).Values
                .SelectMany(marketDataList => marketDataList)
                .ToList();
        }

        private Dictionary<Underlying, List<IMarketData>> GetShiftedMarketDataByUnderlying(IMarketData marketData) {
            return marketData.GetUnderlyings().ToDictionary(underlying => underlying, 
                underlying => new List<IMarketData>() {
                    new ShiftedMarketData(marketData)
                        .ShiftSpot(underlying, 0.99),
                    new ShiftedMarketData(marketData)
                        .ShiftSpot(underlying, 1.01) 
                });
        }

        public ValueWithPrecision GetResult(IMarketData unshiftedMarketData, Dictionary<IMarketData, ValueWithPrecision> resultsByShift) {
            Dictionary<Underlying, List<IMarketData>> marketDataByUnderlying = GetShiftedMarketDataByUnderlying(unshiftedMarketData);
            if (marketDataByUnderlying.Keys.Count == 1) {
                Underlying underlying = marketDataByUnderlying.Keys.First();
                ValueWithPrecision valueDown = resultsByShift[marketDataByUnderlying[underlying][0]];
                ValueWithPrecision valueUp = resultsByShift[marketDataByUnderlying[underlying][1]];
                double deltaValue = (valueUp.Value - valueDown.Value) / (0.02 * unshiftedMarketData.GetSpot(underlying));
                double deltaPrecision = (valueUp.Precision + valueDown.Precision) / 2;
                return new ValueWithPrecision { Value = deltaValue, Precision = deltaPrecision };
            }
            return new ValueWithPrecision { Value = double.NaN, Precision = double.NaN };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
