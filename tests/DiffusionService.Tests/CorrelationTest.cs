using Application;
using Domain;
using MathNet.Numerics.Statistics;

namespace BrownianServices.Tests {
    [TestClass]
    public sealed class CorrelationTest {
        [TestMethod]
        public void TestGeneratedPathsHaveSameLength() {
            Underlying A = new Equity("A", null);
            Underlying B = new Equity("B", null);
            BrowniansConfiguration configuration = new BrowniansConfiguration() {
                Underlyings = new List<Underlying>() { A, B },
                CorrelationMatrix = new Double[,] {
                    { 1.0, 0.7 },
                    { 0.7, 1.0 }
                },
                NumberOfDrawings = 100,
                NumberOfSteps = 100
            };
            BrowniansService browniansService = new();
            BrowniansResult diffusionResult = browniansService.CreateCorrelatedBrownians(configuration);
            Realizations paths_A = diffusionResult.Paths[A];
            Realizations paths_B = diffusionResult.Paths[B];
            Assert.AreEqual(paths_A.Size, paths_B.Size);
        }

        [TestMethod]
        public void TestGeneratedFollowCorrelationMatrix() {
            Underlying A = new Equity("A", null);
            Underlying B = new Equity("B", null);
            BrowniansConfiguration configuration = new BrowniansConfiguration() {
                Underlyings = new List<Underlying>() { A, B },
                CorrelationMatrix = new Double[,] {
                    { 1.0, 0.7 },
                    { 0.7, 1.0 }
                },
                NumberOfDrawings = 50000,
                NumberOfSteps = 100
            };
            BrowniansService browniansService = new();
            BrowniansResult browniansResult = browniansService.CreateCorrelatedBrownians(configuration);
            Realizations paths_A = browniansResult.Paths[A];
            Realizations paths_B = browniansResult.Paths[B];
            Double averageCorrelationMismatch = 0;
            for (int ω = 0; ω < paths_A.Size; ω++) {
                SimulatedPath path_A = paths_A[ω];
                SimulatedPath path_B = paths_B[ω];
                Double correlation = Correlation.Pearson(path_A.Values, path_B.Values);
                averageCorrelationMismatch += Math.Abs(correlation - configuration.CorrelationMatrix[0, 1]);
            }
            averageCorrelationMismatch /= paths_A.Size;
            Assert.IsTrue(averageCorrelationMismatch < 1E-1);
        }

        [TestMethod]
        public void TestGeneratedFollowCorrelationMatrixOnSingleStep() {
            Underlying A = new Equity("A", null);
            Underlying B = new Equity("B", null);
            Double rho = 0.7;
            BrowniansConfiguration configuration = new BrowniansConfiguration() {
                Underlyings = new List<Underlying>() { A, B },
                CorrelationMatrix = new Double[,] {
                    { 1.0, rho },
                    { rho, 1.0 }
                },
                NumberOfDrawings = 1000000,
                NumberOfSteps = 1
            };
            BrowniansService browniansService = new();
            BrowniansResult browniansResult = browniansService.CreateCorrelatedBrownians(configuration);
            Realizations paths_A = browniansResult.Paths[A];
            Realizations paths_B = browniansResult.Paths[B];

            // Concatenate all increments into single arrays for more stable correlation
            List<double> allIncrements_A = new();
            List<double> allIncrements_B = new();
            for (int ω = 0; ω < paths_A.Size; ω++) {
                SimulatedPath path_A = paths_A[ω];
                SimulatedPath path_B = paths_B[ω];
                allIncrements_A.AddRange(path_A.Values);
                allIncrements_B.AddRange(path_B.Values);
            }

            // Compute correlation on aggregated increments
            Double correlation = Correlation.Pearson(allIncrements_A, allIncrements_B);
           
            Assert.AreEqual(rho, correlation, 1E-4, "Correlation should be close");
        }
    }
}
