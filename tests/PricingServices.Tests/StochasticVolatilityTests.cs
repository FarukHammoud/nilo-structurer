using Application;
using Common.Tests;
using Domain;

namespace PricingServices {
    [TestClass]
    public class StochasticVolatilityTests {
        [TestMethod]
        public void StochasticVolatility() {
            Equity MSFT = new Equity("MSFT", Currencies.USD);
            double spot = 100;
            double riskFreeRate = 0.03;

            double initialVariance = 0.04; // 0.2 vol
            double longTermVariance = 0.04; // 0.2 vol
            double meanReversionSpeed = 2.0;
            double vovol = 0.3;
            double correlation = -0.7;

            HestonVolatilityDynamics hestonDynamics = new HestonVolatilityDynamics(meanReversionSpeed, longTermVariance, vovol);
            
            EuropeanCall call = new EuropeanCall() {
                Underlying = MSFT,
                Strike = spot,
                Maturity = DateTime.Today.AddYears(1),
                Currency = Currencies.USD,
            };

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spot))
                .SetVolatilityDynamics(MSFT, hestonDynamics, Math.Sqrt(initialVariance))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate)
                .SetCorrelation(MSFT, new InstantaneousVolatility(MSFT), correlation);

            PricingRequest request = new() {
                Position = [call],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = new ModelConfiguration() {
                    Pricing = new MonteCarlo(),
                    Discounting = new DiscountCurveDiscounting(),
                    Volatility = new StochasticVolatility()
                },
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            Estimate monteCarloResult = results.Get(call, new Premium());
            Heston model = new Heston(meanReversionSpeed, longTermVariance, vovol);
            double timeToMaturity = (call.Maturity - request.PricingDate).TotalDays / 365.0;
            double theoreticalValue = HestonPricer.PriceCall(spot, call.Strike, timeToMaturity, riskFreeRate, vovol, correlation, model);
            StatisticalAssert.IsNormallyDistributed(theoreticalValue, monteCarloResult);
        }
    }
}
