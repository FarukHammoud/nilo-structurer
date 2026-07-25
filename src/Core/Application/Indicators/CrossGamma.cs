using Domain;

namespace Application {
    public class CrossGamma : IIndicator {
        private double _bump;
        public CrossGamma(double bump = 0.01) { 
            _bump = bump;
        }
        public IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate) {
            return GetShiftedMarketDataByUnderlyingPair(marketData, pricingDate).Values
                .SelectMany(marketDataList => marketDataList)
                .ToList();
        }

        private Dictionary<(Underlying First, Underlying Second), List<(IMarketData, DateTime)>> GetShiftedMarketDataByUnderlyingPair(IMarketData marketData, DateTime pricingDate) {
            List<Underlying> underlyings = marketData.Underlyings.ToList();
            int n = underlyings.Count;
            Dictionary<(Underlying First, Underlying Second), List<(IMarketData, DateTime)>> result = new();
            foreach (Underlying first in underlyings) {
                foreach (Underlying second in underlyings) {
                    if (first == second) {
                        continue;
                    }
                    var key = (First: first, Second: second);
                    if (result.ContainsKey(key)) {
                        continue;
                    }
                    result[key] = new List<(IMarketData, DateTime)>() {
                        (new ShiftedMarketData(marketData)
                            .WithShift(first, new SpotShift(1 - _bump))
                            .WithShift(second, new SpotShift(1 - _bump)), pricingDate),
                        (new ShiftedMarketData(marketData)
                            .WithShift(first, new SpotShift(1 - _bump))
                            .WithShift(second, new SpotShift(1 + _bump)), pricingDate),
                        (new ShiftedMarketData(marketData)
                            .WithShift(first, new SpotShift(1 + _bump))
                            .WithShift(second, new SpotShift(1 - _bump)), pricingDate),
                        (new ShiftedMarketData(marketData)
                            .WithShift(first, new SpotShift(1 + _bump))
                            .WithShift(second, new SpotShift(1 + _bump)), pricingDate)
                    };
                }
            }
            return result;
        }

        public IIndicatorResult GetResult(IContract contract, IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), PriceEstimate> resultsByShift) {
            Dictionary<(Underlying First, Underlying Second), List<(IMarketData, DateTime)>> marketDataByUnderlying = GetShiftedMarketDataByUnderlyingPair(unshiftedMarketData, pricingDate);
            ByUnderlyingIndicatorResult result = new();
            foreach ((Underlying First, Underlying Second) in marketDataByUnderlying.Keys) {
                IUnderlyingMarketData firstMarketData = unshiftedMarketData.GetUnderlyingMarketData(First);
                IUnderlyingMarketData secondMarketData = unshiftedMarketData.GetUnderlyingMarketData(Second);
                PriceEstimate valueDownDown = resultsByShift[marketDataByUnderlying[(First, Second)][0]];
                PriceEstimate valueDownUp = resultsByShift[marketDataByUnderlying[(First, Second)][1]];
                PriceEstimate valueUpDown = resultsByShift[marketDataByUnderlying[(First, Second)][2]];
                PriceEstimate valueUpUp = resultsByShift[marketDataByUnderlying[(First, Second)][3]];
                double crossGammaValue = (valueUpUp.Value - valueDownUp.Value - valueUpDown.Value + valueDownDown.Value) / (4 * _bump * _bump * firstMarketData.GetSpot() * secondMarketData.GetSpot());
                double crossGammaPrecision = (valueUpUp.StandardError + valueDownDown.StandardError) / 2;
                result[First] = new Estimate(value : crossGammaValue, standardError: crossGammaPrecision);
            }
            return result;
        }

        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
