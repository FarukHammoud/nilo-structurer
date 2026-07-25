using Application;
using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class BinaryTreeTests {

        [TestMethod]
        public void CallPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);
                

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.BinaryTree,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var binaryTreeResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, binaryTreeResult.Value, 3.09 * binaryTreeResult.StandardError, "The Binary Tree price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);
                

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.BinaryTree,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var binaryTreeResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, binaryTreeResult.Value, 3.09 * binaryTreeResult.StandardError, "The Binary Tree price should be close to the theoretical Black-Scholes price");
        }
    }
}
