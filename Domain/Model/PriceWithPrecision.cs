namespace Domain {
    public record PriceWithPrecision {
        public double Value { get; init; }
        public double Precision { get; init; }
        public Currency Currency { get; init; }
    }
}
