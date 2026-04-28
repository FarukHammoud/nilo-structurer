namespace Domain {
    public record GlobalIndicatorResult : ValueWithPrecision, IIndicatorResult {
        public GlobalIndicatorResult() {
        }
        public GlobalIndicatorResult(ValueWithPrecision valueWithPrecision) {
            Value = valueWithPrecision.Value;
            Precision = valueWithPrecision.Precision;
        }
    }
}
