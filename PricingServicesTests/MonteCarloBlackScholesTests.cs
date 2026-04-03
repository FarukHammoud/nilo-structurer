using Application;
using Domain;
using Domain.Model.Contracts.MultiUnderlyingOptions;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;
using PricerServices.Pricers;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class MonteCarloBlackScholesTests {

        [TestMethod]
        public void DigitalCallPremium() {
            Equity MSFT = new("MSFT");
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
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract.Payoff, 
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void DigitalPutPremium() {
            Equity MSFT = new("MSFT");
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
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract.Payoff,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

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
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract.Payoff,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract.Payoff,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void StraddlePremium() {
            Equity MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            Straddle contract = new() {
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
            double theoreticalPrice = BlackScholes.CallPrice(spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility)
                + BlackScholes.PutPrice(spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility);

            // Price using General Diffusion
            IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> mcPricer = new GeneralDiffusionPricer();
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract.Payoff,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void StranglePremium() {
            Equity MSFT = new("MSFT");
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike1 = 350.0;
            double strike2 = 390.0;
            Strangle contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike1 = strike1,
                Strike2 = strike2,
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
            double theoreticalPrice = BlackScholes.CallPrice(spotPrice, contract.Strike2, timeToMaturity, riskFreeRate, volatility)
                + BlackScholes.PutPrice(spotPrice, contract.Strike1, timeToMaturity, riskFreeRate, volatility);

            // Price using General Diffusion
            IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> mcPricer = new GeneralDiffusionPricer();
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract.Payoff,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void DoubleDigitPremium() {
            Equity MSFT = new("MSFT");
            Equity AAPL = new("AAPL");
            double rho = 0.35;
            double riskFreeRate = 0.0175;
            double volatilityMSFT = 0.34;
            double volatilityAAPL = 0.28;
            double spotMSFT = 370.17;
            double spotAAPL = 255.52;
            double strike1 = 350.0;
            double strike2 = 260.0;
            EuropeanDoubleDigit contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                FirstUnderlying = MSFT,
                SecondUnderlying = AAPL,
                FirstStrike = strike1,
                SecondStrike = strike2
            };
            MarketData marketData = new MarketData()
                .SetSpot(MSFT, spotMSFT)
                .SetSpot(AAPL, spotAAPL)
                .SetDrift(MSFT, riskFreeRate)
                .SetDrift(AAPL, riskFreeRate)
                .SetRiskFreeRate(riskFreeRate)
                .SetVolatility(MSFT, volatilityMSFT)
                .SetVolatility(AAPL, volatilityAAPL)
                .SetCorrelationMatrix(new double[,] {
                    { 1.0, rho },
                    { rho, 1.0 }
                });

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double theoreticalPrice = BlackScholes.DoubleDigital2D(spotMSFT, spotAAPL, strike1, strike2, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity );

            // Price using General Diffusion
            IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> mcPricer = new GeneralDiffusionPricer();
            ValueWithPrecision monteCarloResult = mcPricer.Price(contract.Payoff,
                marketData,
                contract.Maturity,
                DateTime.Today);

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }
    }
}
