using MathNet.Numerics.Statistics;

namespace Domain {
    public record PriceWithPrecision {
        public PriceWithPrecision(IEnumerable<double> values, Currency currency) {
            Value = Statistics.Mean(values);
            Precision = Statistics.StandardDeviation(values) / Math.Sqrt(values.Count());
            Currency = currency;
        }

        public PriceWithPrecision() {}
        public double Value { get; init; }
        public double Precision { get; init; }
        public Currency Currency { get; init; }
    }
}
