using Application;
using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class ForwardStartOptionsTests {

        [TestMethod]
        public void ForwardStartEuropeanCallPremium() {
           Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double alpha = 0.9;
            double riskFreeRate = 0.0465;
            ForwardStartEuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(36),
                Factor = alpha,
                Underlying = MSFT,
                Currency = Currencies.USD,
                StartDate = DateTime.Today.AddMonths(24)
            };
            // Theotetical price using Black-Scholes formula
            double timeToStart = (contract.StartDate - DateTime.Today).TotalYears;
            double timeToMaturity = (contract.Maturity - contract.StartDate).TotalYears;
   
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);


            // Theotetical price using Black-Scholes formula
            double forwardT1 = spotPrice * Math.Exp(riskFreeRate * timeToStart);
            double theoreticalPrice = Math.Exp(-riskFreeRate * timeToStart) * new BlackScholes(OptionType.Call, forwardT1, alpha * forwardT1, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void ForwardStartEuropeanPutPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double alpha = 0.9;
            double riskFreeRate = 0.0465;
            ForwardStartEuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(36),
                Factor = alpha,
                Underlying = MSFT,
                Currency = Currencies.USD,
                StartDate = DateTime.Today.AddMonths(24)
            };
            // Theotetical price using Black-Scholes formula
            double timeToStart = (contract.StartDate - DateTime.Today).TotalYears;
            double timeToMaturity = (contract.Maturity - contract.StartDate).TotalYears;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);


            // Theotetical price using Black-Scholes formula
            double forwardT1 = spotPrice * Math.Exp(riskFreeRate * timeToStart);
            double theoreticalPrice = Math.Exp(-riskFreeRate * timeToStart) * new BlackScholes(OptionType.Put, forwardT1, alpha * forwardT1, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }
    }
}
