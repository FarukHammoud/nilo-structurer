using BrownianServices;
using Domain;

namespace PricerServices {
    public class GeneralDiffusion {

        public static DiffusionResult DiffuseMultiUnderlying(DiffusionConfiguration configuration) {
            Dictionary<Underlying, Realizations> diffusionValues = new();
            int steps = configuration.TimeDiscretization.Count;
            int drawings = configuration.NumberOfDrawings;
            BrowniansConfiguration brownianConfiguration = new() {
                Underlyings = configuration.Underlyings,
                NumberOfSteps = steps,
                CorrelationMatrix = configuration.CorrelationMatrix,
                NumberOfDrawings = configuration.NumberOfDrawings
            };
            BrowniansService browniansService = new();
            BrowniansResult noises = browniansService.CreateCorrelatedBrownians(brownianConfiguration);
            foreach (Underlying underlying in configuration.Underlyings) {
                double drift = configuration.Drifts[underlying];
                double mu = drift;
                double spot = configuration.Spots[underlying];
                DateTime maturity = configuration.TimeDiscretization.LastOrDefault();
                ILocalVolatilityModel volatility = configuration.Volatilities[underlying];
                List<double[]> paths = new();
                for (int eventId = 0; eventId < drawings; eventId++) {
                    double[] path = new double[steps];
                    path[0] = spot;
                    double[] dW = noises.paths[underlying][eventId];
                    for (int step = 1; step < steps; step++) {
                        DateTime T = maturity;
                        DateTime t = configuration.TimeDiscretization[step];
                        double sigma = volatility.getVolatility(path[step - 1], (T - t).TotalDays / 365.0);
                        double dt = (configuration.TimeDiscretization[step] - configuration.TimeDiscretization[step - 1]).TotalDays / 365.0;
                        path[step] = path[step-1] * Math.Exp((mu - 0.5 * sigma * sigma) * dt + sigma * Math.Sqrt(dt) * dW[step]);
                    }
                    paths.Add(path);
                }
                diffusionValues[underlying] = new Realizations { Paths = paths };
            }

            return new DiffusionResult { DiffusionValues = diffusionValues };
        }
    }
}
