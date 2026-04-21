using Application;

namespace PricerServices {
    public interface IPricingEngine {
          Task<PricingResult> RunAsync(PricingRequest request, IProgress<PricingProgress>? progress = null, CancellationToken cancellationToken = default);      
    }
}
    