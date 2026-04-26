using Domain;

namespace Application {
    public class Theta : IIndicator {
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return new List<(IMarketData, DateTime)> { (marketData, pricingDate.AddDays(1)) };
        }

        public ValueWithPrecision GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), ValueWithPrecision> resultsByShift) {
            IList<(IMarketData, DateTime)> marketDatas = GetShiftedMarketData(unshiftedMarketData, pricingDate);
            ValueWithPrecision centralValue = resultsByShift[marketDatas[0]];
            ValueWithPrecision shiftedValue = resultsByShift[marketDatas[1]];
            double theta = (shiftedValue.Value - centralValue.Value);
            double precision = (shiftedValue.Precision + centralValue.Precision) / 2;
            return new ValueWithPrecision { Value = theta, Precision = precision };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
