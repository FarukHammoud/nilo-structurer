using Application;
using Domain;

namespace PricingServices {
    [TestClass]
    public class SwaptionTests {
        [TestMethod]
        public void SwaptionShouldHaveCorrectCriticalRate() {
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            Swaption swaption = new Swaption {
                Notional = 1,
                Swap = new Swap(new ShortRate(Currencies.USD), 0.025, [DateTime.Today.AddMonths(18)]) {
                    Currency = Currencies.USD
                },
                Strike = 0.025,
                Expiry = DateTime.Today.AddMonths(6)
            };
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double criticalRate = SwaptionCriticalRateFinder.FindCriticalRate(swaption, model);
            Assert.AreEqual(0.02418, criticalRate, 1e-5);
        }

        [TestMethod]
        public void SwaptionShouldHaveCorrectPrice() {
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            Swaption swaption = new Swaption {
                Notional = 1,
                Swap = new Swap(new ShortRate(Currencies.USD), 0.025, [DateTime.Today.AddMonths(18)]) {
                    Currency = Currencies.USD
                },
                Strike = 0.025,
                Expiry = DateTime.Today.AddMonths(6)
            };
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double swaptionPrice = SwaptionCriticalRateFinder.Price(swaption, model, DateTime.Today, theta);
            Assert.AreEqual(0.01017, swaptionPrice, 1e-4);
        }
    }
}
