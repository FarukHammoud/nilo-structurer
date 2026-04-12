using Domain;

namespace Application {
    public class Premium : IIndicator {
        public IList<IMarketData> GetShiftedMarketData(IMarketData marketData) => [marketData];

        public ValueWithPrecision GetResult(IMarketData unshiftedMarketData, Dictionary<IMarketData, ValueWithPrecision> resultsByShift) {
            return resultsByShift[unshiftedMarketData];
        }
    }
}
