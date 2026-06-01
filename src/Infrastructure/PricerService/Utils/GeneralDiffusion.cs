using BrownianServices;
using Domain;
using Application;

namespace PricerServices {
    public class GeneralDiffusion {

        public static DiffusionResult DiffuseMultiUnderlying(DiffusionConfiguration configuration) {
            BrowniansResult noises = new BrowniansService()
                .CreateCorrelatedBrownians(configuration.BrowniansConfiguration);
            Dictionary<Underlying, Realizations> diffusionValues = configuration.MarketData.Underlyings
                .ToDictionary(
                    underlying => underlying, 
                    underlying => DiffuseUnderlying(configuration, underlying, noises));
            return new DiffusionResult { DiffusionValues = diffusionValues };
        }

        private static Realizations DiffuseUnderlying(DiffusionConfiguration configuration, Underlying underlying, BrowniansResult noises) {
            int steps = configuration.TimeDiscretization.Count;
            int drawings = configuration.NumberOfDrawings;
            IMarketData marketData = configuration.MarketData;
            IUnderlyingMarketData underlyingMarketData = marketData.GetUnderlyingMarketData(underlying);
            IDriftProvider driftProvider = new DriftProvider();
            double spot = underlyingMarketData.GetSpot();
            DateTime T = configuration.TimeDiscretization.LastOrDefault();
            ILocalVolatilityModel volatility = underlyingMarketData.GetVolatility();
            double μ_adjustment = 0;
            // TODO: Delete jump parameters from configuration, its an underlying thing
            IJumpProcess? jumpProcess = null;
            if (volatility is MertonJumpModel mertonJumpModel) {
                jumpProcess = new PoissonProcess(mertonJumpModel.JumpParameters);
                μ_adjustment -= jumpProcess.GetDrift();
            }
			List<double[]> paths = new();
            Random jumpRandom = new Random();
            for (int ω = 0; ω < drawings; ω++) {
                double[] path = new double[steps];
                path[0] = spot;
                double[] dW = noises.paths[underlying][ω];
                for (int step = 1; step < steps; step++) {
                    DateTime t = configuration.TimeDiscretization[step];
                    DateTime t_1 = configuration.TimeDiscretization[step - 1];
					double μ = driftProvider.GetDrift(underlying, configuration.Currency, marketData, t_1, t);
                    μ += μ_adjustment;
                    double b = underlyingMarketData.GetCarry();
                    double timeToMaturity = (T - t).TotalYears;
                    double σ = volatility.getVolatility(path[step - 1], timeToMaturity);
                    double dt = (t - t_1).TotalYears;
                    path[step] = new LogEulerScheme().Evolve(path[step - 1], timeToMaturity, dt, dW[step], new StochasticDifferentialEquationDefinition(
                        (s, t) => (μ - b) * s,
                        (s, t) => σ * s
                    ));
                    if (jumpProcess != null) {
                        path[step] *= Math.Exp(jumpProcess.Sample(dt, jumpRandom.NextDouble));
                    }
                }
                paths.Add(path);
            }
            return new Realizations { Paths = paths };
        }
    }
}
