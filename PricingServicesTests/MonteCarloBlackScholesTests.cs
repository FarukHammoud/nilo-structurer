using Application;
using Domain;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;
using PricerServices.Pricers;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class MonteCarloBlackScholesTests {

        [TestMethod]
        public void DigitalCallPremium() {
            Underlying MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            BinaryCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            MarketData marketData = new MarketData()
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetRiskFreeRate(riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double theoreticalPrice = BlackScholes.DigitalCallPrice(spotPrice, contract.Strike,timeToMaturity, riskFreeRate, volatility);

            // Price using General Diffusion
            IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> mcPricer = new GeneralDiffusionPricer();
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract, 
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void DigitalPutPremium() {
            Underlying MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            BinaryPut contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            MarketData marketData = new MarketData()
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetRiskFreeRate(riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double theoreticalPrice = BlackScholes.DigitalPutPrice(spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility);

            // Price using General Diffusion
            IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> mcPricer = new GeneralDiffusionPricer();
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CallPremium() {
            Underlying MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCallOption contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            MarketData marketData = new MarketData()
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetRiskFreeRate(riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double theoreticalPrice = BlackScholes.CallPrice(spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility);

            // Price using General Diffusion
            IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> mcPricer = new GeneralDiffusionPricer();
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutPremium() {
            Underlying MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanPutOption contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            MarketData marketData = new MarketData()
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetRiskFreeRate(riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double theoreticalPrice = BlackScholes.PutPrice(spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility);

            // Price using General Diffusion
            IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> mcPricer = new GeneralDiffusionPricer();
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }
    }
}
