using Domain;

namespace Application {
    public class Premium : IIndicator {
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) => [(marketData, pricingDate)];

        public IIndicatorResult GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceWithPrecision> resultsByShift) {
            return new GlobalIndicatorResult(new ValueWithPrecision() {
                Value = resultsByShift[(unshiftedMarketData, pricingDate)].Value,
                Precision = resultsByShift[(unshiftedMarketData, pricingDate)].Precision
            });
        }
        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
