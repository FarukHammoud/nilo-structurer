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

        public ValueWithPrecision GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), ValueWithPrecision> resultsByShift) {
            IList<(IMarketData, DateTime)> marketDatas = GetShiftedMarketData(unshiftedMarketData, pricingDate);
            ValueWithPrecision minusValue = resultsByShift[marketDatas[0]];
            ValueWithPrecision plusValue = resultsByShift[marketDatas[1]];
            double theta = - 365 * (plusValue.Value - minusValue.Value) / (2 * _bump);
            double precision = 365 * (plusValue.Precision + minusValue.Precision) / 2;
            return new ValueWithPrecision { Value = theta, Precision = precision };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
