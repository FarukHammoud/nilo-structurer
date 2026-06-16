using Domain;

namespace Application {
    // TODO: Replace by a sort of dependency injection on client side
    public interface IPricerFactory {
        IPricer<IPathIndependentPayoff> CreatePathIndependentPricer(ModelConfiguration config);
        IPricer<IPathDependentPayoff> CreatePathDependentPricer(ModelConfiguration config);
        IPricerConfiguration CreateConfiguration(PricingRequest request);
    }
}
