using Domain;

namespace Application {
    public class Rho : IIndicator {

        private readonly double _bump;

        public Rho(double bump = 0.01) {
            _bump = bump;
        }
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return [
                (new ShiftedMarketData(marketData).ShiftDiscountRate(-_bump), pricingDate),
                (new ShiftedMarketData(marketData).ShiftDiscountRate(+_bump), pricingDate)];
        }

        public ValueWithPrecision GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), ValueWithPrecision> resultsByShift) {
            IList<(IMarketData, DateTime)> marketDatas = GetShiftedMarketData(unshiftedMarketData, pricingDate);
            ValueWithPrecision minusValue = resultsByShift[marketDatas[0]];
            ValueWithPrecision plusValue = resultsByShift[marketDatas[1]];
            double rho = (plusValue.Value - minusValue.Value) / (2 * _bump);
            double precision = (plusValue.Precision + minusValue.Precision) / 2;
            return new ValueWithPrecision { Value = rho, Precision = precision };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
