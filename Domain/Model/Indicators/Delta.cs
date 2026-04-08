namespace Domain.Model.Indicators {
    public class Delta : IIndicator {
        public List<MarketData> GetShiftedMarketData(MarketData marketData) {
            return new List<MarketData>() { marketData, marketData };
        }

        public ValueWithPrecision GetResult(List<ValueWithPrecision> values) {
            return new ValueWithPrecision() {
                Value = values[1].Value - values[0].Value / 2,
                Precision = values[0].Precision,
            };
        }
    }
}
