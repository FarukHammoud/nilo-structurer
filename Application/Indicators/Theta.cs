using Domain;

namespace Application {
    public class Theta : IIndicator {

        private readonly double _bump;

        public Theta(double bump = 1) {
            _bump = bump;
        }

        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return [
                (marketData, pricingDate.AddDays(-_bump)), 
                (marketData, pricingDate.AddDays(_bump))];
        }

        public IIndicatorResult GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceWithPrecision> resultsByShift) {
            IList<(IMarketData, DateTime)> marketDatas = GetShiftedMarketData(unshiftedMarketData, pricingDate);
            PriceWithPrecision minusValue = resultsByShift[marketDatas[0]];
            PriceWithPrecision plusValue = resultsByShift[marketDatas[1]];
            double theta = - 365 * (plusValue.Value - minusValue.Value) / (2 * _bump);
            double precision = 365 * (plusValue.Precision + minusValue.Precision) / 2;
            return new GlobalIndicatorResult() { Value = theta, Precision = precision };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
