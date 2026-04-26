using Domain;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace BrownianServices {
    public class BrowniansService {

        /// <summary>
        /// This method generates correlated Brownian motion paths based on the provided configuration. 
        /// It first creates independent Brownian motion paths for each underlying asset and then applies the Cholesky decomposition to introduce the specified correlations between them. 
        /// The resulting correlated Brownian paths are returned in a structured format, allowing for further use in simulations or pricing models.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public BrowniansResult CreateCorrelatedBrownians(BrowniansConfiguration configuration) {          
            Dictionary <Underlying, List<Double[]>> correlatedBrownians = new(); 
            Dictionary <Underlying, List<Double[]>> brownians = GetOrCreateBrownians(configuration);
            var RHO = Matrix<double>.Build.DenseOfArray(configuration.CorrelationMatrix);
            var L = RHO.Cholesky().Factor;
            for (int event_id = 0; event_id < configuration.NumberOfDrawings; event_id++) {
                for (int i = 0; i < configuration.Underlyings.Count; i++) {
                    Vector<Double> path = Vector<double>.Build.Dense(configuration.NumberOfSteps);
                    //double[] path = new double[configuration.NumberOfSteps];
                    
                    for (int j = 0; j < configuration.Underlyings.Count; j++) {
                        var src = brownians[configuration.Underlyings[j]][event_id];

                        //double coeff = L[i, j];

                        //for (int k = 0; k < path.Length; k++) {
                        //    path[k] += coeff * src[k];
                        //}
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

        private List<double[]> CreateBrownian(int steps, int drawings, int seed) {
            double[][] brownian = new double[drawings][];
            Parallel.For(0, drawings, ω => {
                var random = new Random(seed + ω);
                var normal = new Normal(0, 1, random);
                var path = new double[steps];
                normal.Samples(path);
                brownian[ω] = path;
            });
            return brownian.ToList();
        }

        private Dictionary<Underlying, List<double[]>> GetOrCreateBrownians(BrowniansConfiguration configuration) {
            return configuration.Underlyings.ToDictionary(
                underlying => underlying,
                underlying => CreateBrownian(configuration.NumberOfSteps, configuration.NumberOfDrawings, underlying.GetHashCode()));
        }
    }
}
