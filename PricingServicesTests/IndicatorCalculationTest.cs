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
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical delta using Black-Scholes
            double theoreticalDelta = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Delta;

            // Price using General Diffusion
            PricingRequest request = new () {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Delta() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            ByUnderlyingIndicatorResult monteCarloResult = (ByUnderlyingIndicatorResult)results[contract][new Delta()];   


            Assert.AreEqual(theoreticalDelta, monteCarloResult.Result[MSFT].Value, 3.09 * monteCarloResult.Result[MSFT].Precision, "The Monte Carlo delta should be close to the theoretical Black-Scholes delta");
        }

        [TestMethod]
        public void GammaBSvsMonteCarlo() {

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
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical gamma using Black-Scholes
            double theoreticalGamma = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Gamma;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Gamma() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            ByUnderlyingIndicatorResult monteCarloResult = (ByUnderlyingIndicatorResult)results[contract][new Gamma()];

            Assert.AreEqual(theoreticalGamma, monteCarloResult.Result[MSFT].Value, 3.09 * monteCarloResult.Result[MSFT].Precision, "The Monte Carlo gamma should be close to the theoretical Black-Scholes gamma");
        }

        [TestMethod]
        public void RhoBSvsMonteCarlo() {

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
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical rho using Black-Scholes
            double theoreticalRho = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Rho;
            IIndicator rho = new Rho();
            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [rho],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][rho];

            Assert.AreEqual(theoreticalRho, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo rho should be close to the theoretical Black-Scholes rho");
        }

        [TestMethod]
        public void ThetaBSvsMonteCarlo() {

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
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical theta using Black-Scholes
            double theoreticalTheta = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Theta;

            // Price using General Diffusion
            IIndicator theta = new Theta();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [theta],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today
            };
            var results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][theta];

            Assert.AreEqual(theoreticalTheta, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo theta should be close to the theoretical Black-Scholes theta");
        }

        [TestMethod]
        public void VegaBSvsMonteCarlo() {
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
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical vega using Black-Scholes
            double theoreticalVega = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Vega;

            // Price using General Diffusion
            IIndicator vega = new Vega();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [vega],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today
            };
            var results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult) results[contract][vega];

            Assert.AreEqual(theoreticalVega, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo theta should be close to the theoretical Black-Scholes theta");
        }
    }
}
