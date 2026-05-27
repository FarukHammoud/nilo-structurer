using Domain;

namespace Application {
    public sealed record PricingResult {
        public required ValueWithPrecision Price { get; init; }
        public required TimeSpan ComputeTime { get; init; }
    }
}
