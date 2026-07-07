namespace Domain {
    public abstract record PricingConfig;
    public sealed record Analytical : PricingConfig;
    public sealed record PDE : PricingConfig;
    public sealed record MonteCarlo : PricingConfig;
    public sealed record LongStaffSchwartz : PricingConfig;
    public sealed record American : PricingConfig; // Generalized LS
    public sealed record BinaryTree : PricingConfig;
}
