using Application;
using Domain;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class BinaryTreeTests {

        [TestMethod]
        public void CallPremium() {
            Equity MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetRiskFreeRate(riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.BinaryTree,
                PricingDate = DateTime.Today
            };
            Dictionary<IContract, Dictionary<IIndicator, ValueWithPrecision>> results = PricingEngine.Run(request);
            ValueWithPrecision binaryTreeResult = results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, binaryTreeResult.Value, 3.09 * binaryTreeResult.Precision, "The Binary Tree price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutPremium() {
            Equity MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetRiskFreeRate(riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double theoreticalPrice = new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.BinaryTree,
                PricingDate = DateTime.Today
            };
            Dictionary<IContract, Dictionary<IIndicator, ValueWithPrecision>> results = PricingEngine.Run(request);
            ValueWithPrecision binaryTreeResult = results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, binaryTreeResult.Value, 3.09 * binaryTreeResult.Precision, "The Binary Tree price should be close to the theoretical Black-Scholes price");
        }
    }
}
