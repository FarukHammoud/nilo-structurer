using Domain;

namespace Application {
    public class Premium : IIndicator {
        public IList<IMarketData> GetShiftedMarketData(IMarketData marketData) => [marketData];

        public ValueWithPrecision GetResult(IMarketData unshiftedMarketData, Dictionary<IMarketData, ValueWithPrecision> resultsByShift) {
            return resultsByShift[unshiftedMarketData];
        }
        public override bool Equals(object? obj) => obj?.GetType() == GetType();

        public override int GetHashCode() {
            return GetType().GetHashCode();
        }
    }
}
