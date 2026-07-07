using Application;
using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class AmericanOptionsTests {

        // Limit Cases that should match the price of a vanilla option
        [TestMethod]
        public void AmericanPutShouldHaveTheSamePremiumInLongstaffSchwartz() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.25;
            double spotPrice = 100.0;
            double riskFreeRate = 0.01;
            AmericanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(18),
                Strike = 95.0,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };

            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);


            // Theotetical price using Barone Adesi Whaley formula
            double theoreticalPrice = BaroneAdesiWhaley.PriceAmerican(OptionType.Put, spotPrice, contract.Strike, riskFreeRate, volatility, timeToMaturity);

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.LongstaffSchwartz,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD,
                NumberOfDrawings = 10000,
            };
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> results = new PricingEngine().Run(request);
            GlobalIndicatorResult lsResult = (GlobalIndicatorResult)results[contract][new Premium()];

            // 8.84 - 8.91
            Assert.AreEqual(theoreticalPrice, lsResult.Value, 3.09 * lsResult.Precision, "");
        }

        [TestMethod]
        public void AmericanPutFlowsShouldHaveTheSamePremiumInAmericanPricer() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.25;
            double spotPrice = 100.0;
            double riskFreeRate = 0.01;
            AmericanPut contract = new() {
                Maturity = DateTime.Today.AddMonths(18),
                Strike = 95.0,
                Underlying = MSFT,
                Currency = Currencies.USD,
            };
            IPayoff Put(DateTime maturity) {
                return new MonoUnderlyingPathIndependentPayoff() {
                    Payoff = (spot) => Math.Max(contract.Strike - spot, 0),
                    PaymentDate = maturity,
                    Maturity = maturity,
                    Currency = Currencies.USD,
                    Underlying = MSFT,
                };
            }
            FlowContract flowsContract = new FlowContract()
                .AddFlow(new AmericanExercisableFlow() {
                    ExerciseParty = ExerciseParty.Holder,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddMonths(18),
                    Payoff = Put(DateTime.Today.AddMonths(18))
                });

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            AmericanPricer pricer = new();
            pricer.Initialize(marketData, Enumerable.Range(0,19).Select(DateTime.Today.AddMonths).ToList());
            PriceWithPrecision price = pricer.Price(flowsContract, DateTime.Today, Currencies.USD);

            // Theotetical price using Barone Adesi Whaley formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = BaroneAdesiWhaley.PriceAmerican(OptionType.Put, spotPrice, contract.Strike, riskFreeRate, volatility, timeToMaturity);

      
            // 8.84 - 8.91
            Assert.AreEqual(theoreticalPrice, price.Value, 3.09 * price.Precision, "");
        }
    }
}
