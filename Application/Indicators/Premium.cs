using Domain;

namespace Application {
    public class Premium : IIndicator {
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) => [(marketData, pricingDate)];

        public IIndicatorResult GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), ValueWithPrecision> resultsByShift) {
            return new GlobalIndicatorResult(resultsByShift[(unshiftedMarketData, pricingDate)]);
        }
        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
