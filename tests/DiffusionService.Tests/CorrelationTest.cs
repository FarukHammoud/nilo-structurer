using Application;
using Domain;
using MathNet.Numerics.LinearAlgebra;
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
            List<Double[]> paths_A = diffusionResult.paths[A];
            List<Double[]> paths_B = diffusionResult.paths[B];
            Assert.AreEqual(paths_A.Count, paths_B.Count);
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
            List<Double[]> paths_A = browniansResult.paths[A];
            List<Double[]> paths_B = browniansResult.paths[B];
            Double averageCorrelationMismatch = 0;
            for (int event_id = 0; event_id < paths_A.Count; event_id++) {
                Double[] path_A = paths_A[event_id];
                Double[] path_B = paths_B[event_id];
                Double correlation = Correlation.Pearson(path_A, path_B);
                averageCorrelationMismatch += Math.Abs(correlation - configuration.CorrelationMatrix[0, 1]);
            }
            averageCorrelationMismatch /= paths_A.Count;
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
            List<Double[]> paths_A = browniansResult.paths[A];
            List<Double[]> paths_B = browniansResult.paths[B];

            // Concatenate all increments into single arrays for more stable correlation
            List<double> allIncrements_A = new();
            List<double> allIncrements_B = new();
            for (int event_id = 0; event_id < paths_A.Count; event_id++) {
                Double[] path_A = paths_A[event_id];
                Double[] path_B = paths_B[event_id];
                allIncrements_A.AddRange(path_A);
                allIncrements_B.AddRange(path_B);
            }

            // Compute correlation on aggregated increments
            Double correlation = Correlation.Pearson(allIncrements_A, allIncrements_B);
           
            Assert.AreEqual(rho, correlation, 1E-4, "Correlation should be close");
        }
    }
}
