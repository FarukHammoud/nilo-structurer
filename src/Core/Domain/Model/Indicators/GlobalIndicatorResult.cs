namespace Domain {
    public record GlobalIndicatorResult : ValueWithPrecision, IIndicatorResult {
        public GlobalIndicatorResult() {
        }
        public GlobalIndicatorResult(ValueWithPrecision valueWithPrecision) {
            Value = valueWithPrecision.Value;
            Precision = valueWithPrecision.Precision;
        }

        public GlobalIndicatorResult(double value, double precision = 0.0) {
            Value = value;
            Precision = precision;
        }
    }
}
