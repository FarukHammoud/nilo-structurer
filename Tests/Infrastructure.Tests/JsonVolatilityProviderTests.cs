using Application;
using Domain;

namespace Infrastructure.Tests {
    [TestClass]
    public sealed class JsonVolatilityProviderTests {
        [TestMethod]
        public async Task GetVolatilitiesAsync_ReturnsCorrectVolatilities() {
            // Arrange
            var path = "Tests/Infrastructure.Tests/Data/option_prices.json";
            var volatilitySurfaceBuilder = new InterpolationVolatilitySurfaceBuilder();
            var provider = new JsonVolatilityProvider(path, volatilitySurfaceBuilder);
            Underlying MSFT = new Equity("MSFT", Currencies.USD);
            Underlying AAPL = new Equity("AAPL", Currencies.USD);

            // Act
            Dictionary<Underlying, ILocalVolatilityModel> volatilities = await provider.GetVolatilitiesAsync(
                [MSFT, AAPL]);

            // Assert
            Assert.AreEqual(301.584991, 301.584991);
        }
    }
}
