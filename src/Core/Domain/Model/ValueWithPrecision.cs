namespace Domain {
    public record ValueWithPrecision {
        public double Value { get; init; }
        public double Precision { get; init; }
    }
}
