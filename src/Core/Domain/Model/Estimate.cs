using MathNet.Numerics.Statistics;

namespace Domain {
    public record Estimate {
        public double Value { get; }
        public double StandardError { get; }
        public Estimate(double value, double standardError = 0.0) {
            Value = value;
            StandardError = standardError;
        }

        public Estimate(IEnumerable<double> values) {
            Value = Statistics.Mean(values);
            StandardError = Statistics.StandardDeviation(values) / Math.Sqrt(values.Count());
        }
    }
}
