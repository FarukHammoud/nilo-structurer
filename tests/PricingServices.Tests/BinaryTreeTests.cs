using Application;
using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class BinaryTreeTests {

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
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);
                

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.BinaryTree,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var binaryTreeResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, binaryTreeResult.Value, 3.09 * binaryTreeResult.StandardError, "The Binary Tree price should be close to the theoretical Black-Scholes price");
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
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Put, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, volatility).Premium;

            // Price using General Diffusion
            PricingRequest request = new() {
                Position = [contract],
                MarketData = marketData,
                Indicators = [new Premium()],
                ModelConfiguration = ModelConfiguration.BinaryTree,
                PricingDate = DateTime.Today,
                PricingCurrency = Currencies.USD
            };
            PricingResults results = new PricingEngine().Run(request);
            var binaryTreeResult = results.Get(contract, new Premium());

            Assert.AreEqual(theoreticalPrice, binaryTreeResult.Value, 3.09 * binaryTreeResult.StandardError, "The Binary Tree price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void DermanKaniCallPremiumFlatVolatility() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.03;
            double spotPrice = 100.0;
            double atmVolatility = 0.1;
            double skew = 0.0;
            double termStructure = 0.0;
            ILocalVolatilityModel volatility = new LinearVolatilityModel(atmVolatility, skew, termStructure, spotPrice);
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddYears(1),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            DermanKaniBinaryTreePricer pricer = new();
            pricer.Initialize(marketData, Enumerable.Range(0, 11).Select(i => DateTime.Today.AddMonths(i)).Append(contract.Maturity).ToList());
            PriceEstimate price = pricer.Price(contract, DateTime.Today, Currencies.USD);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice, contract.Strike, timeToMaturity, riskFreeRate, atmVolatility).Premium;

            Assert.AreEqual(theoreticalPrice, price.Value, 3.09 * price.StandardError, "The Derman-Kani Binary Tree price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void DermanKaniCallPremiumWithSkewedVolatility() {
            Equity MSFT = new("MSFT", Currencies.USD);
            double riskFreeRate = 0.03;
            double spotPrice = 100.0;
            double atmVolatility = 0.1;
            double skew = -0.0005;
            double termStructure = 0.0;
            ILocalVolatilityModel volatility = new LinearVolatilityModel(atmVolatility, skew, termStructure, spotPrice);
            EuropeanCall contract = new() {
                Maturity = DateTime.Today.AddYears(5),
                Strike = spotPrice,
                Underlying = MSFT,
                Currency = Currencies.USD
            };
            MarketData marketData = new MarketData()
                .For<EquityMarketData>(MSFT, md => md
                    .SetSpot(spotPrice)
                    .SetVolatility(volatility))
                .SetRiskFreeRate(Currencies.USD, riskFreeRate);

            DermanKaniBinaryTreePricer pricer = new();
            pricer.Initialize(marketData, Enumerable.Range(0, 11).Select(i => DateTime.Today.AddMonths(i)).Append(contract.Maturity).ToList());
            PriceEstimate price = pricer.Price(contract, DateTime.Today, Currencies.USD);

            // Theotetical price using Black-Scholes formula
            double timeToMaturity = (contract.Maturity - DateTime.Today).TotalYears;
            double a = atmVolatility - skew * spotPrice;
            double theoreticalPrice = new BlackScholes(OptionType.Call, spotPrice + a/skew, contract.Strike + a/skew, timeToMaturity, riskFreeRate, Math.Abs(skew), riskFreeRate*(spotPrice+a/skew)).Premium;

            Assert.AreEqual(theoreticalPrice, price.Value, 3.09 * price.StandardError, "The Derman-Kani Binary Tree price should be close to the theoretical Black-Scholes price");
        }
    }
}
