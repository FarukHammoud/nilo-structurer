using MathNet.Numerics.Statistics;

namespace Domain {
    public record ValueWithPrecision {
        public ValueWithPrecision(IEnumerable<double> values) {
            Value = Statistics.Mean(values);
            Precision = Statistics.StandardDeviation(values);
        }
        public ValueWithPrecision() { }
        public double Value { get; init; }
        public double Precision { get; init; }
    }
}
