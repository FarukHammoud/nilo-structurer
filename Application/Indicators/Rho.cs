using Domain;

namespace Application {
    public class Rho : IIndicator {

        private readonly double _bump;

        public Rho(double bump = 0.01) {
            _bump = bump;
        }
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            IEnumerable<Currency> currencies = [Currencies.USD];//marketData.GetUnderlyings().OfType<Currency>();
            return currencies.SelectMany(currency => new List<(IMarketData, DateTime)>() {
                (new ShiftedMarketData(marketData).ShiftDiscountRate(currency, -_bump), pricingDate),
                (new ShiftedMarketData(marketData).ShiftDiscountRate(currency, +_bump), pricingDate)}).ToList();
        }

        public IIndicatorResult GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceWithPrecision> resultsByShift) {
            IList<(IMarketData, DateTime)> marketDatas = GetShiftedMarketData(unshiftedMarketData, pricingDate);
            PriceWithPrecision minusValue = resultsByShift[marketDatas[0]];
            PriceWithPrecision plusValue = resultsByShift[marketDatas[1]];
            double rho = (plusValue.Value - minusValue.Value) / (2 * _bump);
            double precision = (plusValue.Precision + minusValue.Precision) / 2;
            return new GlobalIndicatorResult() { Value = rho, Precision = precision };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
