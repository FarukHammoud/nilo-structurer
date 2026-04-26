using BrownianServices;
using Domain;
using Application;

namespace PricerServices {
    public class GeneralDiffusion {

        public static DiffusionResult DiffuseMultiUnderlying(DiffusionConfiguration configuration) {
            BrowniansResult noises = new BrowniansService()
                .CreateCorrelatedBrownians(configuration.BrowniansConfiguration);
            Dictionary<Underlying, Realizations> diffusionValues = configuration.Underlyings
                .ToDictionary(
                    underlying => underlying, 
                    underlying => DiffuseUnderlying(configuration, underlying, noises));
            return new DiffusionResult { DiffusionValues = diffusionValues };
        }

        private static Realizations DiffuseUnderlying(DiffusionConfiguration configuration, Underlying underlying, BrowniansResult noises) {
            int steps = configuration.TimeDiscretization.Count;
            int drawings = configuration.NumberOfDrawings;
            double μ = configuration.Drifts[underlying];
            double spot = configuration.Spots[underlying];
            DateTime T = configuration.TimeDiscretization.LastOrDefault();
            ILocalVolatilityModel volatility = configuration.Volatilities[underlying];
            List<double[]> paths = new();
            for (int ω = 0; ω < drawings; ω++) {
                double[] path = new double[steps];
                path[0] = spot;
                double[] dW = noises.paths[underlying][ω];
                for (int step = 1; step < steps; step++) {
                    DateTime t = configuration.TimeDiscretization[step];
                    DateTime t_1 = configuration.TimeDiscretization[step - 1];
                    double timeToMaturity = (T - t).TotalDays / 365.0;
                    double σ = volatility.getVolatility(path[step - 1], timeToMaturity);
                    double dt = (t - t_1).TotalDays / 365.0;
                    path[step] = new LogEulerScheme().Evolve(path[step - 1], timeToMaturity, dt, dW[step], new StochasticDifferentialEquationDefinition(
                        (s, t) => μ * s,
                        (s, t) => σ * s
                    ));
                }
                paths.Add(path);
            }
            return new Realizations { Paths = paths };
        }
    }
}
