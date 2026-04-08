namespace Domain {
    public abstract record PricingConfig;
    public sealed record Analytical : PricingConfig;
    public sealed record PDE : PricingConfig;
    public sealed record MonteCarlo : PricingConfig;
}
