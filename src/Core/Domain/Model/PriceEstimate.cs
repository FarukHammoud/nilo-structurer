namespace Domain {
    public record PriceEstimate : Estimate {
        public Currency Currency { get; init; }
        public PriceEstimate(double value, Currency currency, double standardError = 0.0) : base(value, standardError) {
            Currency = currency;
        }

        public PriceEstimate(Estimate estimate, Currency currency) : base(estimate.Value, estimate.StandardError) {
            Currency = currency;
        }

        public PriceEstimate(IEnumerable<double> values, Currency currency) : base(values) {
            Currency = currency;
        }
    }
}
