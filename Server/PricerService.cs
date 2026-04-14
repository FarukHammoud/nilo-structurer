using Application;
using Domain;

namespace Server {
    public class PricerService {
        private readonly IServiceProvider _serviceProvider;

        public PricerService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public double? Price<T>(T contract) where T : IContract {
            Type pricerType = typeof(IPricer<>).MakeGenericType(contract.GetType());
            dynamic pricer = _serviceProvider.GetRequiredService(pricerType);
            return pricer.Price((dynamic) contract);
        }
    }
}