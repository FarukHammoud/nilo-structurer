using Domain;

namespace Application {
    public interface IPricingEngine {
        // Not yet used, we dont know if it's possible for path dependent contracts
          Task<Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>>> RunAsync(PricingRequest request, IProgress<PricingProgress>? progress = null, CancellationToken cancellationToken = default);      
    }
}
    