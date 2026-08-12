using Application;
using Domain;
using PricingServicesTests;

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
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double riskFreeRate = -Math.Log(discountCurve.GetValue(contract.Maturity)) / timeToMaturity;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetDiscountCurve(Currencies.USD, discountCurve); 
                

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).DigitalCallPrice();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);
                

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).DigitalPutPrice();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
                Currency = Currencies.USD,
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
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
            CashFlows cashFlow = new([
                new CashFlow() { PaymentDate = DateTime.Today, Amount = -spotPrice, Currency = Currencies.USD },
                new CashFlow() { PaymentDate = DateTime.Today.AddMonths(4), Amount = strike, Currency = Currencies.USD}]
            ) {Currency = Currencies.USD};
            Book book = new([ call, put, cashFlow ]);

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);
                


            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { book },
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(book, new Premium());
            Assert.IsLessThan(3.09 * monteCarloResult.StandardError, monteCarloResult.Value, "The Monte Carlo price should be close to 0");
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
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);
                

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium
                + new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);
                

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike2, timeToMaturity, riskFreeRate, volatility).Premium
                + new BlackScholes(OptionType.Put, spotPrice, contract.Strike1, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = Stulz.DoubleDigital2D(spotMSFT, spotAAPL, strike1, strike2, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity );

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
                Underlying = new BestOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = Stulz.CallBestOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
                Underlying = new WorstOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = Stulz.CallWorstOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutBestOfPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            Equity AAPL = new("AAPL", Currencies.USD);
            double rho = 0.5;
            double riskFreeRate = 0.05;
            double volatilityMSFT = 0.2;
            double volatilityAAPL = 0.3;
            double spotMSFT = 100;
            double spotAAPL = 100;
            double strike = 100;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(12),
                Underlying = new BestOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = Stulz.PutBestOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void PutWorstOfPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            Equity AAPL = new("AAPL", Currencies.USD);
            double rho = 0.5;
            double riskFreeRate = 0.05;
            double volatilityMSFT = 0.2;
            double volatilityAAPL = 0.3;
            double spotMSFT = 100;
            double spotAAPL = 100;
            double strike = 100;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(12),
                Underlying = new WorstOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = Stulz.PutWorstOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho, timeToMaturity);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CallWorstOfIsLongCorrelation() {
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
                Underlying = new WorstOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double bump = 0.01;
            double downPrice = Stulz.CallWorstOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho - bump, timeToMaturity);
            double upPrice = Stulz.CallWorstOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho + bump, timeToMaturity);
            double theoreticalSensitivity = (upPrice - downPrice) / (2 * bump);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium(), new CorrelationSensitivity() },
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new CorrelationSensitivity(), MSFT, AAPL);

           
            Assert.IsPositive(monteCarloResult.Value, "Worst-of option is long correlation");
            Assert.AreEqual(theoreticalSensitivity, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo Sensitivity should be close to the theoretical Stulz Sensitivity");
        }

        [TestMethod]
        public void CallBestOfIsShortCorrelation() {
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
                Underlying = new BestOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double bump = 0.01;
            double downPrice = Stulz.CallBestOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho - bump, timeToMaturity);
            double upPrice = Stulz.CallBestOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho + bump, timeToMaturity);
            double theoreticalSensitivity = (upPrice - downPrice) / (2 * bump);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium(), new CorrelationSensitivity() },
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new CorrelationSensitivity(), MSFT, AAPL);


            Assert.IsNegative(monteCarloResult.Value, "Best-of call option is short correlation");
            Assert.AreEqual(theoreticalSensitivity, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo Sensitivity should be close to the theoretical Stulz Sensitivity");
        }

        [TestMethod]
        public void PutWorstOfIsShortCorrelation() {
            Equity MSFT = new("MSFT", Currencies.USD);
            Equity AAPL = new("AAPL", Currencies.USD);
            double rho = 0.5;
            double riskFreeRate = 0.05;
            double volatilityMSFT = 0.2;
            double volatilityAAPL = 0.3;
            double spotMSFT = 100;
            double spotAAPL = 100;
            double strike = 100;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(12),
                Underlying = new WorstOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double bump = 0.01;
            double downPrice = Stulz.PutWorstOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho - bump, timeToMaturity);
            double upPrice = Stulz.PutWorstOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho + bump, timeToMaturity);
            double theoreticalSensitivity = (upPrice - downPrice) / (2 * bump);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium(), new CorrelationSensitivity() },
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new CorrelationSensitivity(), MSFT, AAPL);


            Assert.IsNegative(monteCarloResult.Value, "Worst-of put option is short correlation");
            Assert.AreEqual(theoreticalSensitivity, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo Sensitivity should be close to the theoretical Stulz Sensitivity");
        }

        [TestMethod]
        public void PutBestOfIsLongCorrelation() {
            Equity MSFT = new("MSFT", Currencies.USD);
            Equity AAPL = new("AAPL", Currencies.USD);
            double rho = 0.5;
            double riskFreeRate = 0.05;
            double volatilityMSFT = 0.2;
            double volatilityAAPL = 0.3;
            double spotMSFT = 100;
            double spotAAPL = 100;
            double strike = 100;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(12),
                Underlying = new BestOf([MSFT, AAPL], Currencies.USD),
                Strike = strike,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotMSFT)
                    .SetVolatility(volatilityMSFT))
                .For<EquityMarketData>(AAPL, md => md
                    .SetSpot(spotAAPL)
                    .SetVolatility(volatilityAAPL))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, AAPL, rho);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double bump = 0.01;
            double downPrice = Stulz.PutBestOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho - bump, timeToMaturity);
            double upPrice = Stulz.PutBestOf(spotMSFT, spotAAPL, strike, riskFreeRate, volatilityMSFT, volatilityAAPL, rho + bump, timeToMaturity);
            double theoreticalSensitivity = (upPrice - downPrice) / (2 * bump);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium(), new CorrelationSensitivity() },
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new ConstantVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var monteCarloResult = results.Get(contract, new CorrelationSensitivity(), MSFT, AAPL);


            Assert.IsPositive(monteCarloResult.Value, "Best-of put option is long correlation");
            Assert.AreEqual(theoreticalSensitivity, monteCarloResult.Value, 3.09 * monteCarloResult.StandardError, "The Monte Carlo Sensitivity should be close to the theoretical Stulz Sensitivity");
        }
    }
}
