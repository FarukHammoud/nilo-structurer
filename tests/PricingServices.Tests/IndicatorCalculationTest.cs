using Application;
using Application.Indicators;
using Domain;
using PricingServicesTests;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class IndicatorCalculationTest {

        [TestMethod]
        public void DeltaBSvsMonteCarlo() {

            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetDiscountCurve(Currencies.USD, discountCurve);
                

            // Theotetical delta using Black-Scholes
            double theoreticalDelta = BlackScholesFactory.Create(contract, marketData, DateTime.Today).Delta;

            // Price using General Diffusion
            PricingRequest request = new () {
                Position = [contract],
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Delta() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Delta(), MSFT);   

            Assert.AreEqual(theoreticalDelta, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo delta should be close to the theoretical Black-Scholes delta");
        }

        [TestMethod]
        public void DeltaFXvsMonteCarlo() {

            double volatility = 0.15;
            double spotPrice = 1.17;
            double usdRiskFreeRate = 0.0435;
            double eurRiskFreeRate = 0.0265;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = spotPrice,
                Underlying = CurrencyPairs.EURUSD,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .For<CurrencyPairMarketData>(CurrencyPairs.EURUSD, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, usdRiskFreeRate)
                .SetRiskFreeRate(Currencies.EUR, eurRiskFreeRate);

            // Theotetical delta using Black-Scholes
            double theoreticalDelta = new BlackScholes(
                optionType: OptionType.Call,
                spot: spotPrice,
                strike: contract.Strike,
                timeToMaturity: timeToMaturity,
                riskFreeRate: usdRiskFreeRate,
                volatility: volatility,
                costOfCarry: usdRiskFreeRate - eurRiskFreeRate
                ).Delta;

            // Price using General Diffusion
            IIndicator deltaFx = new DeltaFx();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [deltaFx],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, deltaFx, CurrencyPairs.EURUSD);

            Assert.AreEqual(theoreticalDelta, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo delta fx should be close to the theoretical Black-Scholes delta");
        }

        [TestMethod]
        public void GammaBSvsMonteCarlo() {

            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetDiscountCurve(Currencies.USD, discountCurve);
                

            // Theotetical gamma using Black-Scholes
            double theoreticalGamma = BlackScholesFactory.Create(contract, marketData, DateTime.Today).Gamma;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Gamma() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Gamma(), MSFT);

            Assert.AreEqual(theoreticalGamma, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo gamma should be close to the theoretical Black-Scholes gamma");
        }

        [TestMethod]
        public void RhoBSvsMonteCarlo() {

            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetDiscountCurve(Currencies.USD, discountCurve);
                
            // Theotetical rho using Black-Scholes
            double theoreticalRho = BlackScholesFactory.Create(contract, marketData, DateTime.Today).Rho;
            IIndicator rho = new Rho();
            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [rho],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, rho);

            Assert.AreEqual(theoreticalRho, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo rho should be close to the theoretical Black-Scholes rho");
        }

        [TestMethod]
        public void ThetaBSvsMonteCarlo() {

            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetDiscountCurve(Currencies.USD, discountCurve);
                

            // Theotetical theta using Black-Scholes
            double theoreticalTheta = BlackScholesFactory.Create(contract, marketData, DateTime.Today).Theta;

            // Price using General Diffusion
            IIndicator theta = new Theta();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [theta],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            var results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, theta);

            Assert.AreEqual(theoreticalTheta, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo theta should be close to the theoretical Black-Scholes theta");
        }

        [TestMethod]
        public void VegaBSvsMonteCarlo() {
            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetDiscountCurve(Currencies.USD, discountCurve); 
                

            // Theotetical vega using Black-Scholes
            double theoreticalVega = BlackScholesFactory.Create(contract, marketData, DateTime.Today).Vega;

            // Price using General Diffusion
            IIndicator vega = new Vega();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [vega],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            var results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, vega, MSFT);

            Assert.AreEqual(theoreticalVega, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo vega should be close to the theoretical Black-Scholes vega");
        }

        [TestMethod]
        public void ImpliedVolatilityCall() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, 0.0265);
                

            // Price using General Diffusion
            IIndicator impliedVolatility = new ImpliedVolatility();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [impliedVolatility],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, impliedVolatility);

            Assert.AreEqual(volatility, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo implied volatility should be close to the theoretical Black-Scholes volatility");
        }

        [TestMethod]
        public void ImpliedVolatilityPut() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical delta using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, 0.0265);

            // Price using General Diffusion
            IIndicator impliedVolatility = new ImpliedVolatility();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [impliedVolatility],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            var results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, impliedVolatility);

            Assert.AreEqual(volatility, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo implied volatility should be close to the theoretical Black-Scholes volatility");
        }
    }
}
