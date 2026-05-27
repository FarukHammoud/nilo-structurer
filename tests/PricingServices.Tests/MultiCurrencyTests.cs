using Application;
using Domain;
using Domain.Model.Models;
using PricerServices;

namespace PricingServicesTests {
    [TestClass]
    public sealed class MultiCurrencyTests {

        [TestMethod]
        public void DifferentPricingCurrency() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            double foreignRiskFreeRate = 0.01855;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .For<CurrencyPairMarketData>(CurrencyPairs.USDEUR, md => md
                    .SetSpot(0.86)
                    .SetVolatility(0.1))
                .SetRiskFreeRate(Currencies.EUR, foreignRiskFreeRate)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR,
            };

            double fxRate = marketData.GetFxRate(Currencies.USD, Currencies.EUR);
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice * fxRate, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CompositeForward() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.3;
            double fxVolatility = 0.1;
            double spotPrice = 100.0;
            double domesticRate = 0.01;
            double foreignRate = 0.02;
            double rho = -0.6;
            double fxSpot = 0.1;
            CompositeForward contract = new() {
                Maturity = DateTime.Today.AddMonths(360),
                Strike = 30.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .For<CurrencyPairMarketData>(CurrencyPairs.USDEUR, md => md
                    .SetSpot(fxSpot)
                    .SetVolatility(fxVolatility))
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate)
                .SetCorrelation(MSFT, CurrencyPairs.USDEUR, rho);

            // Theotetical price
            double thoreticalPrice = fxSpot * spotPrice - fxSpot * Math.Exp(-foreignRate * timeToMaturity) * contract.Strike;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR,
                NumberOfDrawings = 250000,
            };

            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(thoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CompositeEuropeanCall() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double fxVolatility = 0.1;
            double spotPrice = 370.17;
            double domesticRate = 0.0265;
            double foreignRate = 0.01855;
            double rho = 0.0;
            double fxSpot = 0.86;
            CompositeEuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(48),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .For<CurrencyPairMarketData>(CurrencyPairs.USDEUR, md => md
                    .SetSpot(fxSpot)
                    .SetVolatility(fxVolatility))
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate)
                .SetCorrelation(MSFT, CurrencyPairs.USDEUR, rho);

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new Composite(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, fxSpot, foreignRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR,
                NumberOfDrawings = 250000,
            };

            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult) results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void QuantoEuropeanCallOption() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double fxVolatility = 0.1;
            double spotPrice = 370.17;
            double domesticRate = 0.0265;
            double foreignRate = 0.01855;
            double rho = 0.2;
            double fxSpot = 0.8547;
            QuantoEuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
                FxRate = fxSpot
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .For<CurrencyPairMarketData>(CurrencyPairs.USDEUR, md => md
                    .SetSpot(fxSpot)
                    .SetVolatility(fxVolatility))
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate)
                .SetCorrelation(MSFT, CurrencyPairs.USDEUR, rho);

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new ReinerQuanto(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, contract.FxRate, domesticRate, foreignRate, volatility, fxVolatility, rho).Premium;
            // Price using General Diffusion
            IIndicator premium = new Premium();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [premium],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR,
            };

            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult) results[contract][premium];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void QuantoEuropeanPutOption() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double fxVolatility = 0.1;
            double spotPrice = 370.17;
            double domesticRate = 0.0265;
            double foreignRate = 0.01855;
            double rho = 0.2;
            double fxSpot = 0.8547;
            QuantoEuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
                FxRate = fxSpot
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .For<CurrencyPairMarketData>(CurrencyPairs.USDEUR, md => md
                    .SetSpot(fxSpot)
                    .SetVolatility(fxVolatility))
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate)
                .SetCorrelation(MSFT, CurrencyPairs.USDEUR, rho);

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new ReinerQuanto(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, contract.FxRate, domesticRate, foreignRate, volatility, fxVolatility, rho).Premium;
            // Price using General Diffusion
            IIndicator premium = new Premium();
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [premium],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR,
            };

            double fxRate = marketData.GetFxRate(Currencies.USD, Currencies.EUR);
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][premium];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }
    }
}
