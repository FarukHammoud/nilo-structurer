namespace Domain {
    public sealed record ModelConfiguration {
        public required DiscountingConfig Discounting { get; init; }
        public required VolatilityConfig Volatility { get; init; }
        public required PricingConfig Pricing { get; init; }

        public static ModelConfiguration BlackScholes => new() {
            Discounting = new ConstantRateDiscounting(),
            Volatility = new ConstantVolatility(),
            Pricing = new Analytical(),
        };

        public static ModelConfiguration LocalVolatilityDiffusion => new() {
            Discounting = new DiscountCurveDiscounting(),
            Volatility = new LocalVolatility(),
            Pricing = new MonteCarlo(),
        };
    }
}
