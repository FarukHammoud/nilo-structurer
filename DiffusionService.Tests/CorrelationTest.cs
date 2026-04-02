using Domain;
using MathNet.Numerics.Statistics;

namespace BrownianServices.Tests {
    [TestClass]
    public sealed class CorrelationTest {
        [TestMethod]
        public void TestGeneratedPathsHaveSameLength() {
            Underlying A = new Underlying("A");
            Underlying B = new Underlying("B");
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
            Underlying A = new Underlying("A");
            Underlying B = new Underlying("B");
            BrowniansConfiguration configuration = new BrowniansConfiguration() {
                Underlyings = new List<Underlying>() { A, B },
                CorrelationMatrix = new Double[,] {
                    { 1.0, 0.7 },
                    { 0.7, 1.0 }
                },
                NumberOfDrawings = 100,
                NumberOfSteps = 1000
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
    }
}
