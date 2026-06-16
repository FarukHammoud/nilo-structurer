using Domain;

namespace Application {
    // TODO: Replace by a sort of dependency injection on client side
    public interface IPricerFactory {
        IPricer CreatePricer(ModelConfiguration config);
        IPricerConfiguration CreateConfiguration(PricingRequest request);
    }
}
