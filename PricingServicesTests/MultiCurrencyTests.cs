using Application;
using Domain;
using MathNet.Numerics.LinearAlgebra;
using PricerServices;

namespace PricingServicesTests {
    [TestClass]
    public sealed class MultiCurrencyTests {

        [TestMethod]
        public void DifferentPricingCurrency() {
            Equity MSFT = new("MSFT");
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
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;

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
        public void CompositeOption() {
            Equity MSFT = new("MSFT");
            double volatility = 0.34;
            double spotPrice = 370.17;
            double riskFreeRate = 0.0265;
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(6),
                Strike = 380.0,
                Underlying = MSFT,
                Currency = Currencies.EUR,
            };
            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalDays / 365.0;

            // we need one discounter by currency
            MarketData marketData = new MarketData()
                .SetUnderlyings([MSFT, CurrencyPairs.EURUSD])
                .SetSpot(MSFT, spotPrice)
                .SetSpot(CurrencyPairs.EURUSD, 1.17)
                .SetVolatility(MSFT, volatility)
                .SetVolatility(CurrencyPairs.EURUSD, 0.1)
                .SetRiskFreeRate(Currencies.USD, riskFreeRate * 0.7)
                .SetRiskFreeRate(Currencies.EUR, riskFreeRate)
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
    }
}
