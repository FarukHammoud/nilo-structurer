using Domain;

namespace Application {
    // TODO: Replace by a sort of dependency injection on client side
    public interface IPricerFactory {
        IPayoffPricer<IPathIndependentPayoff> CreatePathIndependentPricer(ModelConfiguration config);
        IPayoffPricer<IPathDependentPayoff> CreatePathDependentPricer(ModelConfiguration config);
        IPricerConfiguration CreateConfiguration(PricingRequest request);
    }
}
