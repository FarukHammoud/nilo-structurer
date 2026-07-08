using Domain;

namespace Application {
    public class CorrelationSensitivity : IIndicator {
        private double _bump;
        public CorrelationSensitivity(double bump = 0.01) { 
            _bump = bump;
        }
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return GetShiftedMarketDataByUnderlyingPair(marketData, pricingDate).Values
                .SelectMany(marketDataList => marketDataList)
                .ToList();
        }

        private Dictionary<(Underlying First, Underlying Second), List<(IMarketData, DateTime)>> GetShiftedMarketDataByUnderlyingPair(IMarketData marketData, DateTime pricingDate) {
            IList<Underlying> underlyings = marketData.Underlyings;
            int n = underlyings.Count;
            Dictionary<(Underlying First, Underlying Second), List<(IMarketData, DateTime)>> result = new();
            foreach (Underlying second in underlyings) {
                foreach (Underlying first in underlyings) {
                    if (first == second) {
                        break;
                    }
                    var key = (First: first, Second: second);
                    result[key] = new List<(IMarketData, DateTime)>() {
                        (new ShiftedMarketData(marketData)
                            .ShiftCorrelation(first, second, -_bump), pricingDate),
                        (new ShiftedMarketData(marketData)
                            .ShiftCorrelation(first, second, +_bump), pricingDate)
                    };
                }
            }
            return result;
        }

        public IIndicatorResult GetResult(IContract contract, IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceWithPrecision> resultsByShift) {
            Dictionary<(Underlying First, Underlying Second), List<(IMarketData, DateTime)>> marketDataByUnderlying = GetShiftedMarketDataByUnderlyingPair(unshiftedMarketData, pricingDate);
            ByUnderlyingPairIndicatorResult result = new();
            foreach ((Underlying First, Underlying Second) in marketDataByUnderlying.Keys) {
                PriceWithPrecision valueDown = resultsByShift[marketDataByUnderlying[(First, Second)][0]];
                PriceWithPrecision valueUp = resultsByShift[marketDataByUnderlying[(First, Second)][1]];
                double rho = unshiftedMarketData.GetCorrelation(First, Second);
                double correlationSensitivityValue = (valueUp.Value - valueDown.Value) / (2 * _bump);
                double correlationSensitivityPrecision = (valueUp.Precision + valueDown.Precision) / 2;
                result.Result[(First,Second)] = new ValueWithPrecision() { Value = correlationSensitivityValue, Precision = correlationSensitivityPrecision };
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
