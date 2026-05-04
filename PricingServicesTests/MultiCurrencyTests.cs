using Application;
using Domain;
using Domain.Model.Models;
using MathNet.Numerics.LinearAlgebra;
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
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, CurrencyPairs.EURUSD])
                .SetSpot(MSFT, spotPrice)
                .SetSpot(CurrencyPairs.EURUSD, 1.17)
                .SetVolatility(MSFT, volatility)
                .SetVolatility(CurrencyPairs.EURUSD, 0.1)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelationMatrix(Matrix<double>.Build.DenseIdentity(2).ToArray());

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = new List<IContract>() { contract },
                MarketData = marketData,
                Indicators = new List<IIndicator>() { new Premium() },
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR
            };

            double fxRate = marketData.GetFxRate(Currencies.USD, Currencies.EUR);
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice * fxRate, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void CompositeEuropeanCall() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double fxVolatility = 0.1;
            double spotPrice = 370.17;
            double domesticRate = 0.0265;
            double foreignRate = 0.01855;
            double rho = 0.2;
            double fxSpot = 1.17;
            CompositeEuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, CurrencyPairs.EURUSD])
                .SetSpot(MSFT, spotPrice)
                .SetSpot(CurrencyPairs.EURUSD, fxSpot)
                .SetVolatility(MSFT, volatility)
                .SetVolatility(CurrencyPairs.EURUSD, fxVolatility)
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate)
                .SetCorrelationMatrix(new double[2, 2] { { 1, rho }, { rho, 1 } });

            // Theotetical price using Black-Scholes formula
            double theoreticalPrice = new Composite(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, 1/fxSpot, foreignRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR
            };

            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult) results[contract][new Premium()];

            Assert.AreEqual(theoreticalPrice , monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
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
            QuantoEuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
                FxRate = 0.8547
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, CurrencyPairs.EURUSD])
                .SetSpot(MSFT, spotPrice)
                .SetSpot(CurrencyPairs.EURUSD, 1.17)
                .SetVolatility(MSFT, volatility)
                .SetVolatility(CurrencyPairs.EURUSD, fxVolatility)
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate)
                .SetCorrelationMatrix(new double[2,2] {{1, rho}, {rho, 1}});

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
                PricingCurrency = Currencies.EUR
            };

            double fxRate = marketData.GetFxRate(Currencies.USD, Currencies.EUR);
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
            QuantoEuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
                FxRate = 0.8547
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, CurrencyPairs.EURUSD])
                .SetSpot(MSFT, spotPrice)
                .SetSpot(CurrencyPairs.EURUSD, 1.17)
                .SetVolatility(MSFT, volatility)
                .SetVolatility(CurrencyPairs.EURUSD, fxVolatility)
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate)
                .SetCorrelationMatrix(new double[2, 2] { { 1, rho }, { rho, 1 } });

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
                PricingCurrency = Currencies.EUR
            };

            double fxRate = marketData.GetFxRate(Currencies.USD, Currencies.EUR);
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult monteCarloResult = (GlobalIndicatorResult)results[contract][premium];

            Assert.AreEqual(theoreticalPrice, monteCarloResult.Value, 3.09 * monteCarloResult.Precision, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }
    }
}
