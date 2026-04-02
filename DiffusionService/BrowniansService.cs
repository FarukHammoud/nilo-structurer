using Domain;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace BrownianServices {
    public class BrowniansService {

        private Random _random = new Random();

        /// <summary>
        /// This method generates correlated Brownian motion paths based on the provided configuration. 
        /// It first creates independent Brownian motion paths for each underlying asset and then applies the Cholesky decomposition to introduce the specified correlations between them. 
        /// The resulting correlated Brownian paths are returned in a structured format, allowing for further use in simulations or pricing models.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public BrowniansResult CreateCorrelatedBrownians(BrowniansConfiguration configuration) {
            Dictionary <Underlying, List<Double[]>> correlatedBrownians = new(); 
            Dictionary <Underlying, List<Double[]>> brownians = CreateBrownians(configuration);
            var RHO = Matrix<double>.Build.DenseOfArray(configuration.CorrelationMatrix);
            var L = RHO.Cholesky().Factor;
            for (int event_id = 0; event_id < configuration.NumberOfDrawings; event_id++) {
                for (int i = 0; i < configuration.Underlyings.Count; i++) {
                    Vector<Double> path = Vector<double>.Build.Dense(configuration.NumberOfSteps);
                    for (int j = 0; j < configuration.Underlyings.Count; j++) {
                        path += L[i, j] * Vector<double>.Build.DenseOfArray(brownians[configuration.Underlyings[j]][event_id]);
                    }
                    if (!correlatedBrownians.ContainsKey(configuration.Underlyings[i])) {
                        correlatedBrownians[configuration.Underlyings[i]] = new List<double[]>();
                    }
                    correlatedBrownians[configuration.Underlyings[i]].Add(path.ToArray());
                }
            }
            return new BrowniansResult() { 
                paths = correlatedBrownians 
            };
        }

        public List<Double[]> CreateBrownian(int steps, int drawings) {
            List<Double[]> paths = new();
            paths.Add(Normal.Samples(_random, 0, 1).Take(steps).ToArray());
            return paths;
        }

        private Dictionary<Underlying, List<Double[]>> CreateBrownians(BrowniansConfiguration configuration) {
            Dictionary<Underlying, List<Double[]>> brownians = new();
            foreach (Underlying underlying in configuration.Underlyings) {
                brownians.Add(underlying, new List<double[]>());
                for (int i = 0; i < configuration.NumberOfDrawings; i++) {
                    double[] standardBrownian = Normal.Samples(new Random(), 0, 1).Take(configuration.NumberOfSteps).ToArray();
                    brownians[underlying].Add(standardBrownian);
                }
            }
            return brownians;
        }


    }
}
