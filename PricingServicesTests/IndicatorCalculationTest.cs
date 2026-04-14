using Application;
using Domain;
using FixedIncomeServices;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;
using PricerServices.Pricers;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class IndicatorCalculationTest {

        [TestMethod]
        public void DeltaBSvsMonteCarlo() {
            PricingEngine monteCarloEngine = new(ModelConfiguration.LocalVolatilityDiffusion);

            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT");
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .SetUnderlyings(new List<Underlying>() { MSFT })
                .SetSpot(MSFT, spotPrice)
                .SetDrift(MSFT, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical delta using Black-Scholes
            double theoreticalDelta = BlackScholes.CallDelta(spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility);

            // Price using General Diffusion
            PricingRequest request = new () {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Delta() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today
            };
            Dictionary<IContract, Dictionary<IIndicator, ValueWithPrecision>> results = PricingEngine.Run(request);
            ValueWithPrecision monteCarloResult = results[contract][new Delta()];   


            Assert.AreEqual(theoreticalDelta, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }
    }
}
