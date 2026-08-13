using Application;
using Common.Tests;
using Domain;

namespace PricingServicesTests {
    [TestClass]
    public sealed class FixedIncomeTests {

        [TestMethod]
        public void BondPricing() {
            double riskFreeRate = 0.01;
            Bond bond = new() {
                StartDate = DateTime.Today,
                Maturity = DateTime.Today.AddYears(10),
                Coupon = 0.02,
                NextSchedule = date => date.AddYears(1),
                Currency = Currencies.USD,
                Notional = 1000,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, bond.Maturity);

            MarketData marketData = new MarketData()
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [bond],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };

            double theoreticalPrice = (Enumerable.Range(1, 10)
                .Sum(i => bond.Coupon * bond.Notional * Math.Exp(-riskFreeRate * i)) 
                + bond.Notional * Math.Exp(-riskFreeRate * 10));

            PricingResults results = new PricingEngine().Run(request);
            Estimate monteCarloResult = results.Get(bond, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 1E-1, "The Monte Carlo price should be close to the theoretical bond price");
        }

        [TestMethod]
        public void BondDuration() {
            double riskFreeRate = 0.01;
            Bond bond = new() {
                StartDate = DateTime.Today,
                Maturity = DateTime.Today.AddYears(10),
                Coupon = 0.02,
                NextSchedule = date => date.AddYears(1),
                Currency = Currencies.USD,
                Notional = 1000,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, bond.Maturity);

            MarketData marketData = new MarketData()
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [bond],
                MarketData = marketData,
                Indicators = [new Premium(), new Duration()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };

            PricingResults results = new PricingEngine().Run(request);
  
            double price = (Enumerable.Range(1, 10)
                .Sum(i => bond.Coupon * bond.Notional * Math.Exp(-riskFreeRate * i))
                + bond.Notional * Math.Exp(-riskFreeRate * 10));

            double macaulayDuration = (1/price) * (Enumerable.Range(1, 10)
                .Sum(i => i *bond.Coupon * bond.Notional * Math.Exp(-riskFreeRate * i))
                + 10 * bond.Notional * Math.Exp(-riskFreeRate * 10));
            Estimate monteCarloResult = results.Get(bond, new Duration());

            Assert.AreEqual(macaulayDuration, monteCarloResult.Value, 1E-1, "The Monte Carlo price should be close to the theoretical bond duration");
        }

        [TestMethod]
        public void StochasticRatesVasicekBondPricing() {
            ZeroCouponBond bond = new() {
                Maturity = DateTime.Today.AddDays(3 * 365),
                Currency = Currencies.USD,
                Notional = 1000,
            };
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            double spotRate = 0.025;
            MarketData marketData = new MarketData()
                .SetShortRateDynamics(
                    currency: Currencies.USD, 
                    dynamics: new VasicekDynamics(
                        kappa: kappa,
                        sigma: sigma,
                        theta: (x) => theta),
                    spotRate: spotRate)
                .SetRiskFreeRate(Currencies.USD, spotRate);

            // Theoretical Price
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, bond.Maturity);
            double df = new Vasicek(kappa, theta, sigma).DiscountFactor(spotRate, timeToMaturity);
            double price = bond.Notional * df;
            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [bond],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new() {
                    Discounting = new StochasticRatesDiscounting(),
                    Pricing = new MonteCarlo(),
                    Volatility = new LocalVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
                WithControlVariate = false,
                NumberOfDrawings = 50000
            };

            PricingResults results = new PricingEngine().Run(request);
            Estimate monteCarloResult = results.Get(bond, new Premium());
            Assert.AreEqual(price, monteCarloResult.Value, monteCarloResult.StandardError);
        }

        [TestMethod]
        public void StochasticRatesCirBondPricing() {
            ZeroCouponBond bond = new() {
                Maturity = DateTime.Today.AddDays(3 * 365),
                Currency = Currencies.USD,
                Notional = 1000,
            };
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            double spotRate = 0.025;
            MarketData marketData = new MarketData()
                .SetShortRateDynamics(
                    currency: Currencies.USD,
                    dynamics: new CoxIngersollRossDynamics(
                        kappa: kappa,
                        sigma: sigma,
                        theta: (x) => theta),
                    spotRate: spotRate)
                .SetRiskFreeRate(Currencies.USD, spotRate);

            // Theoretical Price
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, bond.Maturity);
            double df = new CoxIngersollRoss(kappa, theta, sigma).DiscountFactor(spotRate, timeToMaturity);
            double price = bond.Notional * df;
            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [bond],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new() {
                    Discounting = new StochasticRatesDiscounting(),
                    Pricing = new MonteCarlo(),
                    Volatility = new LocalVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
                WithControlVariate = false,
            };

            PricingResults results = new PricingEngine().Run(request);
            Estimate monteCarloResult = results.Get(bond, new Premium())   ;
            StatisticalAssert.IsNormallyDistributed(price, monteCarloResult, alpha: 0.001);
        }
    }
}
