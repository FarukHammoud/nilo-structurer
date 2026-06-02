using Application;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tests {
    [TestClass]
    public class MarketDataWiringTests {

        [TestMethod]
        public void LocalContainer_ShouldResolve_CompositeMarketDataService() {
            // 1. Create a fresh, empty DI container builder
            ServiceCollection services = new();
            services.AddTransient<ISpotProvider>(_ => new JsonSpotProvider("Tests/Infrastructure.Tests/Data/spots.json"))
                .AddTransient<IVolatilityProvider>(_ => new JsonVolatilityProvider("Tests/Infrastructure.Tests/Data/volatilities.json"))
                .AddTransient<IRateCurveProvider>(_ => new JsonRateCurveProvider("Tests/Infrastructure.Tests/Data/rates.json"));

            // Wire up the composite service
            services.AddTransient<CompositeMarketDataService>();

            // 3. Build the actual container
            ServiceProvider serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions {
                ValidateOnBuild = true
            });

            // 4. Act: Try to resolve the service
            CompositeMarketDataService? marketDataservice = serviceProvider.GetService<CompositeMarketDataService>();

            // 5. Assert: Verify it successfully resolved the whole tree
            Assert.IsNotNull(marketDataservice);
            var MSFT = new Equity("MSFT", Currencies.USD);
            IMarketData marketData = marketDataservice.GetMarketData(DateTime.Now, [MSFT], [Currencies.USD]).Result;
            double msftSpot = marketData.GetUnderlyingMarketData(MSFT).GetSpot();
            Assert.AreEqual(420.399994, msftSpot);
        }
    }
}
