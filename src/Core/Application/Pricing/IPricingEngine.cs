using Domain;

namespace Application {
    public interface IPricingEngine {
        // Not yet used, we dont know if it's possible for path dependent contracts
        Task<PricingResults> RunAsync(PricingRequest request, IProgress<PricingProgress>? progress = null, CancellationToken cancellationToken = default);
        PricingResults Run(PricingRequest request);
    }
}
    