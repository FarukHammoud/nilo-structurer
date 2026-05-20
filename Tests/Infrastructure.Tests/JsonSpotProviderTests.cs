using Application;
using Domain;

namespace Infrastructure.Tests {
    [TestClass]
    public sealed class JsonSpotProviderTests {
        [TestMethod]
        public async Task GetSpotsAsync_ReturnsCorrectSpots() {
            // Arrange
            var path = "Tests/Infrastructure.Tests/Data/spots.json";
            var provider = new JsonSpotProvider(path);
            Underlying MSFT = new Equity("MSFT", Currencies.USD);
            Underlying AAPL = new Equity("AAPL", Currencies.USD);

            // Act
            Dictionary<Underlying, double> spots = await provider.GetSpotsAsync(
                [MSFT, AAPL, CurrencyPairs.EURUSD]);

            // Assert
            Assert.AreEqual(301.584991, spots[AAPL]);
            Assert.AreEqual(420.399994, spots[MSFT]);
            Assert.AreEqual(1.163061, spots[CurrencyPairs.EURUSD]);

        }
    }
}
