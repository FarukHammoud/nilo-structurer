using Application;
using Domain;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class BarrierOptionsTests {

        // Limit Cases that should match the price of a vanilla option
        [TestMethod]
        public void CallUpAndOutInfinityBarrierShouldMatchVanillaPrice() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            CallUpAndOut contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = 1e9,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        // Limit Cases that should match the price of a vanilla option
        [TestMethod]
        public void PutDownAndInZeroBarrierShouldHaveZeroValue() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            PutDownAndIn contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = 1e-6,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(0, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        // Normal Cases
        [TestMethod]
        public void CallUpAndIn() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            CallUpAndIn contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = 400.0,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, contract.BarrierLevel, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Up, BarrierType.KnockIn);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CallUpAndOut() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double barrierLevel = 450.0;
            double adjustedBarrier = barrierLevel * Math.Exp(+ 0.5826 * volatility * Math.Sqrt(1/365.0));
            CallUpAndOut contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = barrierLevel,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, adjustedBarrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Up, BarrierType.KnockOut);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CallDownAndIn() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double barrierLevel = spotPrice * 0.9;
            double adjustedBarrier = barrierLevel * Math.Exp(-0.5826 * volatility * Math.Sqrt(1 / 365.0));
            CallDownAndIn contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = barrierLevel,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, adjustedBarrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Down, BarrierType.KnockIn);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CallDownAndOut() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double barrierLevel = spotPrice * 0.9;
            double adjustedBarrier = barrierLevel * Math.Exp(-0.5826 * volatility * Math.Sqrt(1 / 365.0));
            CallDownAndOut contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = barrierLevel,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, adjustedBarrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Down, BarrierType.KnockOut);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutUpAndIn() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double barrierLevel = 400.0;
            double adjustedBarrier = barrierLevel * Math.Exp(0.5826 * volatility * Math.Sqrt(1 / 365.0));
            PutUpAndIn contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = 400.0,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, adjustedBarrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Up, BarrierType.KnockIn);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutUpAndOut() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double barrierLevel = 450.0;
            double adjustedBarrier = barrierLevel * Math.Exp(+0.5826 * volatility * Math.Sqrt(1 / 365.0));
            PutUpAndOut contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = barrierLevel,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, adjustedBarrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Up, BarrierType.KnockOut);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutDownAndIn() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double barrierLevel = spotPrice * 0.9;
            double adjustedBarrier = barrierLevel * Math.Exp(-0.5826 * volatility * Math.Sqrt(1 / 365.0));
            PutDownAndIn contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = barrierLevel,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, adjustedBarrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Down, BarrierType.KnockIn);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutDownAndOut() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double barrierLevel = spotPrice * 0.9;
            double adjustedBarrier = barrierLevel * Math.Exp(-0.5826 * volatility * Math.Sqrt(1 / 365.0));
            PutDownAndOut contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                BarrierLevel = barrierLevel,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            ReinerRubinstein model = new(spotPrice, contract.Strike, adjustedBarrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Down, BarrierType.KnockOut);
            double theoreticalPrice = model.Price();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
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
