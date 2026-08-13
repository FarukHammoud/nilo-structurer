using Application;
using Common.Tests;
using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public class MertonDiffusionJumpsTests {
        [TestMethod]
        public void CallPremiumWithJumps() {
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
            JumpParameters jumpParameters = new(λ: 1, μJ: -0.1, σJ: 0.15);
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(new MertonJumpModel(jumpParameters, volatility)))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = new MertonDiffusionJumps(jumpParameters, OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };
            PricingResults results = new PricingEngine().Run(request);
            Estimate monteCarloResult = results.Get(contract, new Premium());

            StatisticalAssert.IsNormallyDistributed(theoreticalPrice, monteCarloResult, alpha: 0.001);
        }

        [TestMethod]
        public void PutPremiumWithJumps() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.0175;
            double volatility = 0.34;
            double spotPrice = 370.17;
            EuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(3),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            JumpParameters jumpParameters = new(λ: 1, μJ: -0.1, σJ: 0.15);
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(new MertonJumpModel(jumpParameters, volatility)))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = new Actual365().YearFraction(DateTime.Today, contract.Maturity);
            double theoreticalPrice = new MertonDiffusionJumps(jumpParameters, OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium();

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
            };
            PricingResults results = new PricingEngine().Run(request);
            Estimate monteCarloResult = results.Get(contract, new Premium());

            StatisticalAssert.IsNormallyDistributed(theoreticalPrice, monteCarloResult, alpha: 0.001);
        }
    }
}
