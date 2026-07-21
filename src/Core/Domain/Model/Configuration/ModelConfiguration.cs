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

        public static ModelConfiguration BinaryTree => new() {
            Discounting = new DiscountCurveDiscounting(),
            Volatility = new ConstantVolatility(),
            Pricing = new BinaryTree(),
        };

        public static ModelConfiguration LongstaffSchwartz => new() {
            Discounting = new DiscountCurveDiscounting(),
            Volatility = new ConstantVolatility(),
            Pricing = new LongStaffSchwartz(),
        };

        public static ModelConfiguration American => new() {
            Discounting = new DiscountCurveDiscounting(),
            Volatility = new ConstantVolatility(),
            Pricing = new American(),
        };

        public static ModelConfiguration StochasticRates => new() {
            Discounting = new StochasticRatesDiscounting(),
            Volatility = new ConstantVolatility(),
            Pricing = new American(),
        };
    }
}
