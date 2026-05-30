using Domain;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace PricingServices.Tests {
    [TestClass]
    public class FiniteDifferenceBsPdeSolverTests {

        [TestMethod]
        public void ThomasAlgorithmsWorks() {
            var A = Matrix<double>.Build.DenseOfArray(new double[,] {
                { 4, -1, 0, 0 },
                { -1, 4, -1, 0 },
                { 0, -1, 4, -1 },
                { 0, 0, -1, 3 }
            });
            var b = Vector<double>.Build.DenseOfArray(new double[] { 15, 10, 10, 10 });
            var expected = Vector<double>.Build.DenseOfArray(new double[] { 5, 5, 5, 5 });
            var result = Vector<double>.Build.Dense(b.Count);
            A.ThomasSolve(b, result);
            Assert.IsTrue(result.ToList().ListAlmostEqual(expected.ToList(), 1E-6));
        }

        [TestMethod]
        public void EuropeanCallMatches() {
            double S = 100;
            double K = 95;
            double σ = 0.25;
            double r = 0.01;
            double T = 1.5;
            double q = 0.00;
            FiniteDifferenceBsPdeSolver solver = new FiniteDifferenceBsPdeSolver();
            Func<double, double> payoff = S => Math.Max(S - K, 0);
            double price = solver.PriceEuropean(payoff, S, σ, r, T, q);
            double theoreticalPrice = new BlackScholes(OptionType.Call, S, K, T, r, σ).Premium;
            Assert.AreEqual(price, theoreticalPrice, 1E-2);
        }

        [TestMethod]
        public void EuropeanPutMatches() {
            double S = 100;
            double K = 95;
            double σ = 0.25;
            double r = 0.01;
            double T = 1.5;
            double q = 0.00;
            FiniteDifferenceBsPdeSolver solver = new FiniteDifferenceBsPdeSolver();
            Func<double, double> payoff = S => Math.Max(K - S, 0);
            double price = solver.PriceEuropean(payoff, S, σ, r, T, q);
            double theoreticalPrice = new BlackScholes(OptionType.Put, S, K, T, r, σ).Premium;
            Assert.AreEqual(price, theoreticalPrice, 1E-2);
        }

        [TestMethod]
        public void AmericanCallWithoutDividendsHasSamePriceAsEuropean() {
            double S = 100;
            double K = 95;
            double σ = 0.25;
            double r = 0.01;
            double T = 1.5;
            double q = 0.00;
            FiniteDifferenceBsPdeSolver solver = new FiniteDifferenceBsPdeSolver();
            Func<double, double> payoff = S => Math.Max(S - K, 0);
            double price = solver.PriceAmerican(payoff, S, σ, r, T, q);
            double theoreticalPrice = new BlackScholes(OptionType.Call, S, K, T, r, σ).Premium;
            Assert.AreEqual(price, theoreticalPrice, 1E-2);
        }

        [TestMethod]
        public void AmericanCallIsMoreExpansiveThanEuropean() {
            double S = 100;
            double K = 100;
            double σ = 0.25;
            double r = 0.02;
            double T = 1.5;
            double q = 0.01;
            FiniteDifferenceBsPdeSolver solver = new FiniteDifferenceBsPdeSolver();
            Func<double, double> payoff = S => Math.Max(S - K, 0);
            double priceAmerican = solver.PriceAmerican(payoff, S, σ, r, T, q);
            double priceEuropean = solver.PriceEuropean(payoff, S, σ, r, T, q);
            Assert.IsGreaterThan(priceEuropean, priceAmerican);
        }

        [TestMethod]
        public void AmericanPutIsMoreExpansiveThanEuropean() {
            double S = 100;
            double K = 95;
            double σ = 0.25;
            double r = 0.02;
            double T = 1.5;
            double q = 0.01;
            FiniteDifferenceBsPdeSolver solver = new FiniteDifferenceBsPdeSolver();
            Func<double, double> payoff = S => Math.Max(K - S, 0);
            double priceAmerican = solver.PriceAmerican(payoff, S, σ, r, T, q);
            double priceEuropean = solver.PriceEuropean(payoff, S, σ, r, T, q);
            Assert.IsGreaterThan(priceEuropean, priceAmerican);
        }

        [TestMethod]
        public void AmericanCallMatchesBaroneAdesiWhaley() {
            double S = 100;
            double K = 95;
            double σ = 0.25;
            double r = 0.01;
            double T = 1.5;
            double q = 0.00;
            FiniteDifferenceBsPdeSolver solver = new FiniteDifferenceBsPdeSolver();
            Func<double, double> payoff = S => Math.Max(S - K, 0);
            double price = solver.PriceAmerican(payoff, S, σ, r, T, q);
            double theoreticalPrice = BaroneAdesiWhaley.PriceAmerican(OptionType.Call, S, K, r, σ, T);
            Assert.AreEqual(price, theoreticalPrice, 1E-2);
        }

        [TestMethod]
        public void AmericanPutMatchesBaroneAdesiWhaley() {
            double S = 100;
            double K = 95;
            double σ = 0.25;
            double r = 0.01;
            double T = 1.5;
            double q = 0.00;
            FiniteDifferenceBsPdeSolver solver = new FiniteDifferenceBsPdeSolver();
            Func<double, double> payoff = S => Math.Max(K - S, 0);
            double price = solver.PriceAmerican(payoff, S, σ, r, T, q);
            double theoreticalPrice = BaroneAdesiWhaley.PriceAmerican(OptionType.Put, S, K, r, σ, T);
            Assert.AreEqual(price, theoreticalPrice, 1E-1);
        }
    }
}
