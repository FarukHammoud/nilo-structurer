namespace Domain {
    public record GlobalIndicatorResult : Estimate, IIndicatorResult {
        public GlobalIndicatorResult(Estimate valueWithPrecision) : base(valueWithPrecision.Value, valueWithPrecision.StandardError) {}

        public GlobalIndicatorResult(double value, double precision = 0.0) : base(value, precision) {}
    }
}
