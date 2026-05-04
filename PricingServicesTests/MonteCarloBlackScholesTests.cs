using Application;
using Domain;
using FixedIncomeServices;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;
using PricerServices.Pricers;
using System.Diagnostics.Contracts;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class MonteCarloBlackScholesTests {

        [TestMethod]
        public void DigitalCallPremium() {
            Curve discountCurve = ZeroCouponBootstrapper.GetDiscountCurve(ExampleCurves.ExampleSwapCurve);
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            BinaryCall contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetVolatility(MSFT, volatility)
                .SetDiscountCurve(Currencies.USD, discountCurve)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).DigitalCallPrice();

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
        public void DigitalPutPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            BinaryPut contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).DigitalPutPrice();

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
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
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
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

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
        public void CallPutParity() {
            // c = p + S0 - K*exp(-rT)

            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.1;
            EuropeanCall call = new() {
                Maturity = DateTime.Today.AddMonths(4),
                Strike = strike,    
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            EuropeanPut put = new() {
                Maturity = DateTime.Today.AddMonths(4),
                Strike = strike,
                Underlying = MSFT,
                Notional = -1.0,
                Currency = Currencies.USD,
            };
            CashFlow cashFlow = new([ 
                Tuple.Create(DateTime.Today, -spotPrice), 
                Tuple.Create(DateTime.Today.AddMonths(4), strike) ]) {
                 Currency = Currencies.USD
            };
            Book book = new([ call, put, cashFlow ]);

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());


            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { book },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[book][new Premium()];
            Assert.IsLessThan(3.09 * monteCarloResult.Precision, monteCarloResult.Value, "The Monte Carlo price should be close to 0");
        }

        [TestMethod]
        public void StraddlePremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            Straddle contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium
                + new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

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
        public void StranglePremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike1 = 350.0;
            double strike2 = 390.0;
            Strangle contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike1 = strike1,
                Strike2 = strike2,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT])
                .SetSpot(MSFT, spotPrice)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatility)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(1).ToArray());

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike2, timeToMaturity, riskFreeRate, volatility).Premium
                + new BlackScholes(OptionType.Put, spotPrice, contract.Strike1, timeToMaturity, riskFreeRate, volatility).Premium;

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
        public void DoubleDigitPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            Equity AAPL = new("AAPL", Currencies.USD);
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
                SecondStrike = strike2,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, AAPL])
                .SetSpot(MSFT, spotMSFT)
                .SetSpot(AAPL, spotAAPL)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatilityMSFT)
                .SetVolatility(AAPL, volatilityAAPL)
                .SetCorrelationMatrix(new double[,] {
                    { 1.0, rho },
                    { rho, 1.0 }
                });

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = Stulz.DoubleDigital2D(spotMSFT, spotAAPL, strike1, strike2, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity );

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
        public void CallBestOfPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            Equity AAPL = new("AAPL", Currencies.USD);
            double rho = 0.5;
            double riskFreeRate = 0.05;
            double volatilityMSFT = 0.2;
            double volatilityAAPL = 0.3;
            double spotMSFT = 100;
            double spotAAPL = 100;
            double strike = 100;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(12),
                Underlying = new BestOf(MSFT, AAPL, Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, AAPL])
                .SetSpot(MSFT, spotMSFT)
                .SetSpot(AAPL, spotAAPL)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatilityMSFT)
                .SetVolatility(AAPL, volatilityAAPL)
                .SetCorrelationMatrix(new double[,] {
                    { 1.0, rho },
                    { rho, 1.0 }
                });

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = Stulz.CallBestOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity);

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
        public void CallWorstOfPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            Equity AAPL = new("AAPL", Currencies.USD);
            double rho = 0.5;
            double riskFreeRate = 0.05;
            double volatilityMSFT = 0.2;
            double volatilityAAPL = 0.3;
            double spotMSFT = 100;
            double spotAAPL = 100;
            double strike = 100;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(12),
                Underlying = new WorstOf(MSFT, AAPL, Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, AAPL])
                .SetSpot(MSFT, spotMSFT)
                .SetSpot(AAPL, spotAAPL)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetVolatility(MSFT, volatilityMSFT)
                .SetVolatility(AAPL, volatilityAAPL)
                .SetCorrelationMatrix(new double[,] {
                    { 1.0, rho },
                    { rho, 1.0 }
                });

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = Stulz.CallWorstOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity);

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
