using Domain;

namespace Application {
    public class Rho : IIndicator {

        private readonly double _bump;

        public Rho(double bump = 0.01) {
            _bump = bump;
        }
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return [(marketData, pricingDate), (new ShiftedMarketData(marketData).ShiftDiscountRate(_bump), pricingDate)];
        }

        public ValueWithPrecision GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), ValueWithPrecision> resultsByShift) {
            IList<(IMarketData, DateTime)> marketDatas = GetShiftedMarketData(unshiftedMarketData, pricingDate);
            ValueWithPrecision centralValue = resultsByShift[marketDatas[0]];
            ValueWithPrecision shiftedValue = resultsByShift[marketDatas[1]];
            double rho = (shiftedValue.Value - centralValue.Value) / _bump;
            double precision = (shiftedValue.Precision + centralValue.Precision) / 2;
            return new ValueWithPrecision { Value = rho, Precision = precision };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
