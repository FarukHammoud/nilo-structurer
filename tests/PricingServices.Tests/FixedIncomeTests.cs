using Application;
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
            double timeToMaturity = (bond.Maturity - DateTime.Today).TotalYears;

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

            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[bond][new Premium()];

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
            double timeToMaturity = (bond.Maturity - DateTime.Today).TotalYears;

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

            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
  
            double price = (Enumerable.Range(1, 10)
                .Sum(i => bond.Coupon * bond.Notional * Math.Exp(-riskFreeRate * i))
                + bond.Notional * Math.Exp(-riskFreeRate * 10));

            double macaulayDuration = (1/price) * (Enumerable.Range(1, 10)
                .Sum(i => i *bond.Coupon * bond.Notional * Math.Exp(-riskFreeRate * i))
                + 10 * bond.Notional * Math.Exp(-riskFreeRate * 10));
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[bond][new Duration()];

            Assert.AreEqual(macaulayDuration, monteCarloResult.Value, 1E-1, "The Monte Carlo price should be close to the theoretical bond duration");
        }
    }
}
