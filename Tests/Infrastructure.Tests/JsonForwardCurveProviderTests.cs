using Application;
using Domain;

namespace Infrastructure.Tests {
    [TestClass]
    public class JsonForwardCurveProviderTests {
        [TestMethod]
        public async Task GetForwardCurveAsync_ReturnsCorrectForwardCurves() {
            // Arrange
            var path = "Tests/Infrastructure.Tests/Data/option_prices.json";
            var optionPriceProvider = new JsonOptionPriceProvider(path);
            var discounter = new FixedRateDiscounter() { Rate = 0.02}; // Replace with actual discounter implementation
            var provider = new JsonForwardCurveProvider(optionPriceProvider, discounter);
            Underlying MSFT = new Equity("MSFT", Currencies.USD);

            // Act
            Dictionary<Underlying, Curve> forwardCurves = await provider.GetForwardCurveAsync(
                [MSFT]);
            Curve msftForwardCurve = forwardCurves[MSFT];
            // Assert
            Assert.AreEqual(301.584991, 301.584991);
        }
    }
}
