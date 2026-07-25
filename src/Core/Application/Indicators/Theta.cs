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

        public IIndicatorResult GetResult(IContract contract, IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceEstimate> resultsByShift) {
            IList<(IMarketData, DateTime)> marketDatas = GetShiftedMarketData(unshiftedMarketData, pricingDate);
            PriceEstimate minusValue = resultsByShift[marketDatas[0]];
            PriceEstimate plusValue = resultsByShift[marketDatas[1]];
            double theta = - 365 * (plusValue.Value - minusValue.Value) / (2 * _bump);
            double precision = 365 * (plusValue.StandardError + minusValue.StandardError) / 2;
            return new GlobalIndicatorResult(value:theta, precision:precision);
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode(); 
        }
    }
}
