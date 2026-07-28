

using Application;

namespace PricingServices.Tests {
    [TestClass]
    public class BrownianBridgeTests {
        [TestMethod]
        public void BrownianBridgeExpectedMaxIsCorrectInAlignedCase() {
            double sigma = 1.0;
            double spacing = 0.01;
            BrownianBridge brownianBridge = new BrownianBridge(0, 0, spacing, sigma);
            double maxExpectation = brownianBridge.ExpectedMax();
            Assert.AreEqual(sigma * Math.Sqrt(Math.PI * spacing / 8), maxExpectation);
        }

        [TestMethod]
        public void BrownianBridgeExpectedMinIsCorrectInAlignedCase() {
            double sigma = 1.0;
            double spacing = 0.01;
            BrownianBridge brownianBridge = new BrownianBridge(0, 0, spacing, sigma);
            double minExpectation = brownianBridge.ExpectedMin();
            Assert.AreEqual(-sigma * Math.Sqrt(Math.PI * spacing / 8), minExpectation);
        }

        [TestMethod]
        public void BrownianBridgeMaxCdfReturnsZeroWhenValueIsLessThanMax() {
            double sigma = 1.0;
            double spacing = 0.01;
            BrownianBridge brownianBridge = new BrownianBridge(5, 8, spacing, sigma);
            double maxCdf = brownianBridge.MaxCdf(7.9);
            Assert.AreEqual(0, maxCdf);
        }

        [TestMethod]
        public void BrownianBridgeMaxCdfReturnsOneWhenValueIsBig() {
            double sigma = 1.0;
            double spacing = 0.01;
            BrownianBridge brownianBridge = new BrownianBridge(5, 8, spacing, sigma);
            double maxCdf = brownianBridge.MaxCdf(8 + 10* sigma);
            Assert.AreEqual(1, maxCdf);
        }

        [TestMethod]
        public void BrownianBridgeMinCdfReturnsOneWhenValueIsMoreThanMin() {
            double sigma = 1.0;
            double spacing = 0.01;
            BrownianBridge brownianBridge = new BrownianBridge(5, 8, spacing, sigma);
            double minCdf = brownianBridge.MinCdf(5.1);
            Assert.AreEqual(1, minCdf);
        }

        [TestMethod]
        public void BrownianBridgeMinCdfReturnsZeroWhenValueIsSmall() {
            double sigma = 1.0;
            double spacing = 0.01;
            BrownianBridge brownianBridge = new BrownianBridge(5, 8, spacing, sigma);
            double minCdf = brownianBridge.MinCdf(0.01);
            Assert.AreEqual(0, minCdf);
        }
    }
}
