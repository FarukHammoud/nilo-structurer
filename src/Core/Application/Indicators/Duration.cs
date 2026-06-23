using Application.Contracts;
using Domain;

namespace Application {
    public class Duration : IIndicator {
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return [(marketData, pricingDate)];
        }

        public IIndicatorResult GetResult(IContract contract, IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceWithPrecision> resultsByShift) {
            if (contract is not Bond bond) {
                return null;
            }
            double price = resultsByShift[(unshiftedMarketData, pricingDate)].Value;
            CashFlows cashFlows = bond.CashFlows;
            DateTime startDate = bond.StartDate;
            IEnumerable<DateTime> dates = cashFlows.Dates;
            IEnumerable<double> values = cashFlows.Values;
            double duration = 0;
            for (int i = 0; i < dates.Count(); i++) {
                DateTime date = dates.ElementAt(i);
                double value = values.ElementAt(i);
                double discountedValue = unshiftedMarketData.GetDiscounter(bond.Currency).GetDiscountFactor(date, startDate) * value;
                duration += discountedValue * (date - startDate).TotalYears;
            }
            duration /= price;
            return new GlobalIndicatorResult { Value = duration };
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
