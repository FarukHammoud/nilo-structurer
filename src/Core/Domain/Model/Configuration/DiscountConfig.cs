namespace Domain {
    public abstract record DiscountingConfig;
    public sealed record ConstantRateDiscounting : DiscountingConfig;
    public sealed record DiscountCurveDiscounting : DiscountingConfig;
}
