using Application;
using Domain;

namespace PricingServices {
    [TestClass]
    public class CorrelationSensitivityTests {
        [TestMethod]
        public void ForwardComboCorrelationSensitivity() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double volatility = 0.34;
            double spot = 382.83;
            double fxVolatility = 0.12;
            double fxSpot = 0.88;
            double correlation = 0.35;
            double domesticRate = 0.0435;
            double foreignRate = 0.0265;

            Converse converse = new() {
                Maturity = DateTime.Today.AddMonths(12),
                Strike = spot,
                Underlying = MSFT,
                Currency = Currencies.USD
            };

            QuantoConverse quantoConverse = new() {
                Notional = -1.0,
                Maturity = DateTime.Today.AddMonths(12),
                Strike = spot,
                Underlying = MSFT,
                Currency = Currencies.EUR,
                FxRate = fxSpot
            };

            Book combo = new Book([converse, quantoConverse]);

            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spot)
                    .SetVolatility(volatility))
                .For<CurrencyPairMarketData>(CurrencyPairs.USDEUR, md => md
                    .SetSpot(fxSpot)
                    .SetVolatility(fxVolatility))
                .SetCorrelation(MSFT, CurrencyPairs.USDEUR, correlation)
                .SetRiskFreeRate(Currencies.USD, foreignRate)
                .SetRiskFreeRate(Currencies.EUR, domesticRate);

            // Price using General Diffusion
            IIndicator correlationSensitivity = new CorrelationSensitivity(bump:0.1);
            PricingRequest request = new() {
                Position = [converse, quantoConverse, combo],
                MarketData = marketData,
                Indicators = [correlationSensitivity, new Premium(), new Delta()],
                ModelConfiguration = ModelConfiguration.LocalVolatilityDiffusion,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.EUR
            };
            PricingResults results = new PricingEngine().Run(request);
            var conversePremium = results.Get(converse, new Premium()).Value;
            var converseQuantoPremium = results.Get(quantoConverse, new Premium()).Value;
            var converseDelta = results.Get(converse, new Delta(), MSFT).Value;
            var converseQuantoDelta = results.Get(quantoConverse, new Delta(), MSFT).Value;
            var converseSensitivity = results.Get(converse, correlationSensitivity, MSFT, CurrencyPairs.USDEUR).Value;
            double converseQuantoSensitivity = results.Get(quantoConverse, correlationSensitivity, MSFT, CurrencyPairs.USDEUR).Value;
            double comboSensitivity = results.Get(combo, correlationSensitivity, MSFT, CurrencyPairs.USDEUR).Value;
            Assert.IsNegative(comboSensitivity, "The Combo should have negative correlation Sensitivity");
        }
    }
}
