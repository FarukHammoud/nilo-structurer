using Application;
using Common.Tests;
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
                Swap = new Swap() {
                    DayCounter = new Actual365(),
                    FloatingRate = new ShortRate(Currencies.USD),
                    FixedRate = 0.025,
                    Dates = [DateTime.Today.AddMonths(18)],
                    Currency = Currencies.USD
                },
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
                Swap = new Swap() {
                    DayCounter = new Actual365(),
                    FloatingRate = new ShortRate(Currencies.USD),
                    FixedRate = 0.025,
                    Dates = [DateTime.Today.AddMonths(18)],
                    Currency = Currencies.USD
                },
                Expiry = DateTime.Today.AddMonths(6)
            };
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double swaptionPrice = SwaptionCriticalRateFinder.Price(swaption, model, DateTime.Today, theta);
            Assert.AreEqual(0.01017, swaptionPrice, 1e-4);
        }

        [TestMethod]
        public void SwapFloatingLegPriceInAmericanPricerWithVasicekModel() {
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            double spotRate = 0.03;
            Swap onlyFloatingSwap = new Swap() {
                DayCounter = new Actual365(),
                FloatingRate = new ShortRate(Currencies.USD),
                FixedRate = 0.0,
                Dates = [DateTime.Today.AddMonths(18)],
                Notional = 10000, // so premium is in bps
                Currency = Currencies.USD
            };

            MarketData marketData = new MarketData()
                .SetShortRateDynamics(
                    currency: Currencies.USD,
                    dynamics: new VasicekDynamics(
                        kappa: kappa,
                        sigma: sigma,
                        theta: (x) => theta),
                    spotRate: spotRate)
                .SetRiskFreeRate(Currencies.USD, spotRate);

            PricingRequest request = new() {
                Position = [onlyFloatingSwap],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.StochasticRates,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };
            var results = new PricingEngine().Run(request);
            var onlyFloatingSwapPrice = results.Get(onlyFloatingSwap, new Premium());
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double P_0_T1 = model.DiscountFactor(spotRate, new Actual365().YearFraction(DateTime.Today, DateTime.Today.AddMonths(18)));
            double P_0_T0 = model.DiscountFactor(spotRate, new Actual365().YearFraction(DateTime.Today, DateTime.Today.AddMonths(6)));
            double theoreticalFloatingLegPrice = onlyFloatingSwap.Notional * (P_0_T0 - P_0_T1);
            StatisticalAssert.IsNormallyDistributed(theoreticalFloatingLegPrice, onlyFloatingSwapPrice, alpha: 0.001);
        }

        [TestMethod]
        public void SwapPriceInAmericanPricerWithVasicekModel() {
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            double spotRate = 0.03;
            Swap swap = new Swap() {
                DayCounter = new Actual365(),
                FloatingRate = new ShortRate(Currencies.USD),
                FixedRate = 0.025,
                Dates = [DateTime.Today.AddMonths(18)],
                Notional = 10000, // so premium is in bps
                Currency = Currencies.USD
            };

            MarketData marketData = new MarketData()
                .SetShortRateDynamics(
                    currency: Currencies.USD,
                    dynamics: new VasicekDynamics(
                        kappa: kappa,
                        sigma: sigma,
                        theta: (x) => theta),
                    spotRate: spotRate)
                .SetRiskFreeRate(Currencies.USD, spotRate);

            PricingRequest request = new() {
                Position = [swap],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.StochasticRates,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };
            var results = new PricingEngine().Run(request);
            var swapPrice = results.Get(swap, new Premium());
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double P_0_T1 = model.DiscountFactor(spotRate, new Actual365().YearFraction(DateTime.Today, DateTime.Today.AddMonths(18)));
            double P_0_T0 = model.DiscountFactor(spotRate, new Actual365().YearFraction(DateTime.Today, DateTime.Today.AddMonths(6)));
            double theoreticalFixedLegPrice = swap.Notional * (P_0_T1 * swap.FixedRate);
            double theoreticalFloatingLegPrice = swap.Notional * (P_0_T0 - P_0_T1);
            double theoreticalSwapPrice = theoreticalFloatingLegPrice - theoreticalFixedLegPrice;
            StatisticalAssert.IsNormallyDistributed(theoreticalSwapPrice, swapPrice, alpha: 0.001);
        }

        [TestMethod]
        public void SwaptionShouldPriceInAmericanPricerWithVasicekModel() {
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            double spotRate = 0.03;
            Swap swap = new Swap() {
                DayCounter = new Actual365(),
                FloatingRate = new ShortRate(Currencies.USD),
                FixedRate = 0.025,
                Dates = [DateTime.Today.AddMonths(18)],
                Notional = 10000, // so premium is in bps
                Currency = Currencies.USD
            };
            Swaption swaption = new Swaption {
                Swap = swap,
                Expiry = DateTime.Today.AddMonths(6)
            };

            MarketData marketData = new MarketData()
                .SetShortRateDynamics(
                    currency: Currencies.USD,
                    dynamics: new VasicekDynamics(
                        kappa: kappa,
                        sigma: sigma,
                        theta: (x) => theta),
                    spotRate: spotRate)
                .SetRiskFreeRate(Currencies.USD, spotRate);

            PricingRequest request = new() {
                Position = [swaption],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.StochasticRates,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };
            PricingResults results = new PricingEngine().Run(request);
            var swaptionPrice = results.Get(swaption, new Premium());
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double theoreticalSwaptionPrice = SwaptionCriticalRateFinder.Price(swaption, model, DateTime.Today, currentRate: spotRate);
            StatisticalAssert.IsNormallyDistributed(theoreticalSwaptionPrice, swaptionPrice, alpha: 0.001);
        }
    }
}
