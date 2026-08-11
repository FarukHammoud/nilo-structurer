using Application;
using Domain;
using System.Diagnostics;

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
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
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
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
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

        [TestMethod]
        public void DermanKaniCallPremiumFlatVolatility() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.03;
            double spotPrice = 100.0;
            double atmVolatility = 0.1;
            double skew = 0.0;
            double termStructure = 0.0;
            ILocalVolatilityModel volatility = new LinearVolatilityModel(atmVolatility, skew, termStructure, spotPrice);
            
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddYears(1),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.DermanKani,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };

            PricingResults results = (new PricingEngine()).Run(request);
            var dermanKaniResult = results.Get(contract, new Premium());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, atmVolatility).Premium;

            Debug.WriteLine($"Theoretical Price: {theoreticalPrice}");
            Debug.WriteLine($"Derman-Kani Price: {dermanKaniResult.Value}");
            Debug.WriteLine($"Derman-Kani Standard Error: {dermanKaniResult.StandardError}");
            Assert.AreEqual(theoreticalPrice, dermanKaniResult.Value, 3.09 * dermanKaniResult.StandardError, "The Derman-Kani Binary Tree price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        // Result on Displaced Diffusion Option Pricing, Rubinstein, 1983
        public void DermanKaniCallPremiumWithSkewedVolatilityShouldMatchBlackScholesShiftedPrice() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.03;
            double spotPrice = 100.0;
            double volatility = 0.1;
            double shift = -20;
            ILocalVolatilityModel volatilityModel = new InverseLinearVolatilityModel(volatility, shift, riskFreeRate);
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(18),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatilityModel))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.DermanKani,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };

            PricingResults results = (new PricingEngine()).Run(request);
            var dermanKaniResult = results.Get(contract, new Premium());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice + shift * Math.Exp(-riskFreeRate * timeToMaturity), contract.Strike + shift, timeToMaturity, riskFreeRate, volatility).Premium;

            Debug.WriteLine($"Theoretical Price: {theoreticalPrice}");
            Debug.WriteLine($"Derman-Kani Price: {dermanKaniResult.Value}");
            Debug.WriteLine($"Derman-Kani Standard Error: {dermanKaniResult.StandardError}");
            Assert.AreEqual(theoreticalPrice, dermanKaniResult.Value, 3.09 * dermanKaniResult.StandardError, "The Derman-Kani Binary Tree price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        /// It will never work, we are mismatching local and implied volatilities.
        /// Should we convert it for the diffuser?
        public void DermanKaniCallPremiumWithSkewedVolatilityVsDiffusion() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.03;
            double spotPrice = 100.0;
            double volatility = 0.1;
            double shift = -20;

            IImpliedVolatilityModel volatilityModel = 
                new InverseLinearVolatilityModel(volatility, shift, riskFreeRate);

            ILocalVolatilityModel localVolatilityModel =
                new DupireLocalVolatilityModel(volatilityModel, new FixedRateDiscounter() { Rate = riskFreeRate });
            
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(18),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(localVolatilityModel))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.DermanKani,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };

            PricingResults results = (new PricingEngine()).Run(request);
            var dermanKaniResult = results.Get(contract, new Premium());

            // Price using General Diffusion
            PricingRequest diffusionRequest = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults diffusionResults = new PricingEngine().Run(diffusionRequest);
            Estimate diffusionResult = diffusionResults.Get(contract, new Premium());
 
            Assert.AreEqual(diffusionResult.Value, dermanKaniResult.Value, 3.09 * diffusionResult.StandardError, "The Derman-Kani Binary Tree price should be close to the diffusion price");
        }
    }
}
