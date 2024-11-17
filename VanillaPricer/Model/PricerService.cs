using Application;
using Domain;

namespace VanillaPricer {
    public class PricerService {
        private readonly IServiceProvider _serviceProvider;

        public PricerService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public double? Price<T>(T contract) where T : VanillaContract {
            IPricer<T> pricer = (IPricer<T>)_serviceProvider.GetRequiredService(typeof(IPricer<T>));
            return pricer.Price(contract);
        }
    }
}