using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public sealed class ReinerRubinsteinTests {

        [TestMethod]
        public void ReinerRubinsteinCallUpReflectionTest() {
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.05;
            double riskFreeRate = 0.0265;
            double timeToMaturity = 0.25;
            double barrier = spotPrice * 1.1;
            // Theotetical price using Black-Scholes formula
            ReinerRubinstein ki_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Up, BarrierType.KnockIn);
            double kiPrice = ki_model.Price();

            ReinerRubinstein ko_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Up, BarrierType.KnockOut);
            double koPrice = ko_model.Price();

            double bsPrice = new BlackScholes(OptionType.Call, spotPrice, strike, timeToMaturity, riskFreeRate, volatility).Premium;
            Assert.AreEqual(bsPrice, kiPrice + koPrice, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void ReinerRubinsteinCallDownReflectionTest() {
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.05;
            double riskFreeRate = 0.0265;
            double timeToMaturity = 0.25;
            double barrier = spotPrice * 1.1;
            // Theotetical price using Black-Scholes formula
            ReinerRubinstein ki_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Down, BarrierType.KnockIn);
            double kiPrice = ki_model.Price();

            ReinerRubinstein ko_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Down, BarrierType.KnockOut);
            double koPrice = ko_model.Price();

            double bsPrice = new BlackScholes(OptionType.Call, spotPrice, strike, timeToMaturity, riskFreeRate, volatility).Premium;
            Assert.AreEqual(bsPrice, kiPrice + koPrice, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void ReinerRubinsteinPutUpReflectionTest() {
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.05;
            double riskFreeRate = 0.0265;
            double timeToMaturity = 0.25;
            double barrier = spotPrice * 1.1;
            // Theotetical price using Black-Scholes formula
            ReinerRubinstein ki_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Up, BarrierType.KnockIn);
            double kiPrice = ki_model.Price();

            ReinerRubinstein ko_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Up, BarrierType.KnockOut);
            double koPrice = ko_model.Price();

            double bsPrice = new BlackScholes(OptionType.Put, spotPrice, strike, timeToMaturity, riskFreeRate, volatility).Premium;
            Assert.AreEqual(bsPrice, kiPrice + koPrice, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void ReinerRubinsteinPutDownReflectionTest() {
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.05;
            double riskFreeRate = 0.0265;
            double timeToMaturity = 0.25;
            double barrier = spotPrice * 1.1;
            // Theotetical price using Black-Scholes formula
            ReinerRubinstein ki_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Down, BarrierType.KnockIn);
            double kiPrice = ki_model.Price();

            ReinerRubinstein ko_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Down, BarrierType.KnockOut);
            double koPrice = ko_model.Price();

            double bsPrice = new BlackScholes(OptionType.Put, spotPrice, strike, timeToMaturity, riskFreeRate, volatility).Premium;
            Assert.AreEqual(bsPrice, kiPrice + koPrice, "The Monte Carlo price should be close to the theoretical Black-Scholes price");
        }

        [TestMethod]
        public void ReinerRubinsteinCallUpAndOutZeroValue() {
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.05;
            double riskFreeRate = 0.0265;
            double timeToMaturity = 0.25;
            double barrier = strike;
            // Theotetical price using Black-Scholes formula
            ReinerRubinstein ko_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Up, BarrierType.KnockOut);
            double koPrice = ko_model.Price();

            Assert.AreEqual(0.0, koPrice, 1e-6, "The price of a call up-and-out with barrier equal to strike should be zero");
        }

        [TestMethod]
        public void ReinerRubinsteinPutKnockInZeroZeroValue() {
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.05;
            double riskFreeRate = 0.0265;
            double timeToMaturity = 0.25;
            double barrier = 1e-6;
            // Theotetical price using Black-Scholes formula
            ReinerRubinstein ki_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Put, BarrierDirection.Down, BarrierType.KnockIn);
            double kiPrice = ki_model.Price();

            Assert.AreEqual(0.0, kiPrice, 1e-6, "The price of a put down-and-in with barrier equal to 0 should be zero");
        }

        [TestMethod]
        public void ReinerRubinsteinCallKnockOutInfinityMatchesBS() {
            double volatility = 0.34;
            double spotPrice = 370.17;
            double strike = spotPrice * 1.05;
            double riskFreeRate = 0.0265;
            double timeToMaturity = 0.25;
            double barrier = 1e9;
            // Theotetical price using Black-Scholes formula
            ReinerRubinstein ko_model = new(spotPrice, strike, barrier, timeToMaturity, riskFreeRate, volatility, 0.0, OptionType.Call, BarrierDirection.Up, BarrierType.KnockOut);
            double koPrice = ko_model.Price();
            double bsPrice = new BlackScholes(OptionType.Call, spotPrice, strike, timeToMaturity, riskFreeRate, volatility).Premium;

            Assert.AreEqual(bsPrice, koPrice, 1e-6, "The price of a call up-and-out with barrier at infinity should match the Black-Scholes price");
        }
    }
}
