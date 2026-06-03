using Domain;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace Application {
    public class BrowniansService {
        public static Random Random = new Random();

        /// <summary>
        /// This method generates correlated Brownian motion paths based on the provided configuration. 
        /// It first creates independent Brownian motion paths for each underlying asset and then applies the Cholesky decomposition to introduce the specified correlations between them. 
        /// The resulting correlated Brownian paths are returned in a structured format, allowing for further use in simulations or pricing models.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public BrowniansResult CreateCorrelatedBrownians(BrowniansConfiguration configuration) {
            int n = configuration.Underlyings.Count;
            int steps = configuration.NumberOfSteps;
            int drawings = configuration.NumberOfDrawings;

            var L = Matrix<double>.Build
                .DenseOfArray(configuration.CorrelationMatrix)
                .Cholesky().Factor;

            // pre-generate independent N(0,1) increments per underlying
            var independent = GetOrCreateBrownians(configuration);

            // pre-initialize output
            var correlated = configuration.Underlyings.ToDictionary(
                u => u,
                _ => new List<double[]>(drawings));

            for (int ω = 0; ω < drawings; ω++) {
                for (int i = 0; i < n; i++) {
                    double[] path = new double[steps];
                    for (int j = 0; j < n; j++) {
                        double Lij = L[i, j];
                        if (Lij == 0.0) {
                            continue; // Cholesky is lower-triangular, skip zeros
                        }
                        double[] src = independent[configuration.Underlyings[j]][ω];
                        for (int k = 0; k < steps; k++) {
                            path[k] += Lij * src[k];
                        }
                    }
                    correlated[configuration.Underlyings[i]].Add(path);
                }
            }

            return new BrowniansResult() { paths = correlated };
        }

        private double[][] CreateBrownian(int steps, int drawings, Random random) {
            double[][] brownian = new double[drawings][];
            for (int ω = 0; ω < drawings; ω++) {
                var normal = new Normal(0, 1, random);
                var path = new double[steps];
                normal.Samples(path);
                brownian[ω] = path;
            }
            return brownian;
        }

        private Dictionary<Underlying, double[][]> GetOrCreateBrownians(BrowniansConfiguration configuration) {
            return configuration.Underlyings
                .Select(underlying => (underlying, index:configuration.Underlyings.IndexOf(underlying)))
                .ToDictionary(
                    tuple => tuple.underlying,
                    tuple => CreateBrownian(configuration.NumberOfSteps, configuration.NumberOfDrawings, new Random(42 + 1000 * tuple.index)));
        }
    }
}
