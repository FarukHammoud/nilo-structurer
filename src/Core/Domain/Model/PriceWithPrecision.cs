namespace Domain {
    public record PriceWithPrecision {
        public double Value { get; init; }
        public double Precision { get; init; }
        public required Currency Currency { get; init; }
    }
}
