using Application;
using Domain;
using FixedIncomeServices;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;
using PricerServices.Pricers;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class PathDependentDiffusionTests {

        [TestMethod]
        public void DigitalCallPremium() {
            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT");
            double volatility = 0.34;
            double spotPrice = 370.17;
            BinaryCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = BlackScholes.DigitalCallPrice(spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility);

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
