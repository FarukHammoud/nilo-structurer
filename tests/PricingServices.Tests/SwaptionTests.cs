using Application;
using Domain;
using MathNet.Numerics.Statistics;

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

        [TestMethod]
        public void SwaptionShouldPriceInAmericanPricerWithVasicekModel() {
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            double spotRate = 0.03;
            Swaption swaption = new Swaption {
                Notional = 1,
                Swap = new Swap(new ShortRate(Currencies.USD), 0.025, [DateTime.Today.AddMonths(18)]) {
                    Currency = Currencies.USD
                },
                Strike = 0.025,
                Expiry = DateTime.Today.AddMonths(6)
            };
            AmericanPricer pricer = new AmericanPricer();
            MarketData marketData = new MarketData()
                .SetShortRateDynamics(
                    currency: Currencies.USD,
                    dynamics: new VasicekDynamics(
                        kappa: kappa,
                        sigma: sigma,
                        theta: (x) => theta),
                    spotRate: spotRate)
                .SetRiskFreeRate(Currencies.USD, spotRate);

            pricer.Initialize(marketData, [DateTime.Today, DateTime.Today.AddMonths(6), DateTime.Today.AddMonths(18)]);
            PriceWithPrecision swaptionPrice = pricer.Price(swaption, DateTime.Today, Currencies.USD);
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double theoreticalSwaptionPrice = SwaptionCriticalRateFinder.Price(swaption, model, DateTime.Today, theta);
            Assert.AreEqual(theoreticalSwaptionPrice, swaptionPrice.Value, 1e-4);
        }

    }
}
