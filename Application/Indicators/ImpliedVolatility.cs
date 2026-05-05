using Domain;

namespace Application.Indicators {
    public class ImpliedVolatility : IIndicator {
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) => [(marketData, pricingDate)];

        public IIndicatorResult GetResult(IContract contract, IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceWithPrecision> resultsByShift) {
            double price = resultsByShift[(unshiftedMarketData, pricingDate)].Value;
            if (contract is VanillaContract vanilla) {
                return new GlobalIndicatorResult(new ValueWithPrecision() {
                    Value = BlackScholesFactory.Create(vanilla, unshiftedMarketData, pricingDate).GetImpliedVolatility(price),
                    Precision = 0.01
                });
            }
            return new GlobalIndicatorResult(new ValueWithPrecision() {
                Value = 0,
                Precision = 0
            }); // TODO : find another approch to handle this
        }
        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
