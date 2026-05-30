using Application;

namespace PricerServices {
    public interface IPricingEngine {
        // Not yet used, we dont know if it's possible for path dependent contracts
          Task<PricingResult> RunAsync(PricingRequest request, IProgress<PricingProgress>? progress = null, CancellationToken cancellationToken = default);      
    }
}
    