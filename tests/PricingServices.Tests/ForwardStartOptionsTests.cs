using Application;
using Common.Tests;
using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class ForwardStartOptionsTests {

        [TestMethod]
        public void ForwardStartEuropeanCallPremium() {
           Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double alpha = 0.9;
            double riskFreeRate = 0.0465;
            ForwardStartEuropeanCall contract = new() {
                Maturity = DateTime.Today.AddMonths(36),
                Factor = alpha,
                Underlying = MSFT,
                Currency = Currencies.USD,
                StartDate = DateTime.Today.AddMonths(24)
            };
            // Theotetical price using Black-Scholes formula
            double timeToStart = new Actual365().YearFraction(DateTime.Today, contract.StartDate);
            double timeToMaturity = new Actual365().YearFraction(contract.StartDate, contract.Maturity);

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);


            // Theotetical price using Black-Scholes formula
            double forwardT1 = spotPrice * Math.Exp(riskFreeRate * timeToStart);
            double theoreticalPrice = Math.Exp(-riskFreeRate * timeToStart) * new BlackScholes(OptionType.Call, forwardT1, alpha * forwardT1, timeToMaturity, riskFreeRate, volatility).Premium;

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
            Estimate monteCarloResult = results.Get(contract, new Premium());

            StatisticalAssert.IsNormallyDistributed(theoreticalPrice, monteCarloResult, alpha: 0.001);
        }

        [TestMethod]
        public void ForwardStartEuropeanPutPremium() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spotPrice = 370.17;
            double alpha = 0.9;
            double riskFreeRate = 0.0465;
            ForwardStartEuropeanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(36),
                Factor = alpha,
                Underlying = MSFT,
                Currency = Currencies.USD,
                StartDate = DateTime.Today.AddMonths(24)
            };
            // Theotetical price using Black-Scholes formula
            double timeToStart = new Actual365().YearFraction(DateTime.Today, contract.StartDate);
            double timeToMaturity = new Actual365().YearFraction(contract.StartDate, contract.Maturity);

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);


            // Theotetical price using Black-Scholes formula
            double forwardT1 = spotPrice * Math.Exp(riskFreeRate * timeToStart);
            double theoreticalPrice = Math.Exp(-riskFreeRate * timeToStart) * new BlackScholes(OptionType.Put, forwardT1, alpha * forwardT1, timeToMaturity, riskFreeRate, volatility).Premium;

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
            Estimate monteCarloResult = results.Get(contract, new Premium());

            StatisticalAssert.IsNormallyDistributed(theoreticalPrice, monteCarloResult, alpha: 0.001);
        }
    }
}
