namespace Domain {
    public abstract record VolatilityConfig;
    public sealed record ConstantVolatility : VolatilityConfig;
    public sealed record LocalVolatility : VolatilityConfig;
    public sealed record StochasticVolatility : VolatilityConfig;
}
