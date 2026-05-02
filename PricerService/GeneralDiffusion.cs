using BrownianServices;
using Domain;
using Application;

namespace PricerServices {
    public class GeneralDiffusion {

        public static DiffusionResult DiffuseMultiUnderlying(DiffusionConfiguration configuration) {
            BrowniansResult noises = new BrowniansService()
                .CreateCorrelatedBrownians(configuration.BrowniansConfiguration);
            Dictionary<Underlying, Realizations> diffusionValues = configuration.MarketData.GetUnderlyings()
                .ToDictionary(
                    underlying => underlying, 
                    underlying => DiffuseUnderlying(configuration, underlying, noises));
            return new DiffusionResult { DiffusionValues = diffusionValues };
        }

        private static Realizations DiffuseUnderlying(DiffusionConfiguration configuration, Underlying underlying, BrowniansResult noises) {
            int steps = configuration.TimeDiscretization.Count;
            int drawings = configuration.NumberOfDrawings;
            IUnderlyingMarketData underlyingMarketData = configuration.MarketData.GetUnderlyingMarketData(underlying);
            double spot = underlyingMarketData.GetSpot();
            DateTime T = configuration.TimeDiscretization.LastOrDefault();
            ILocalVolatilityModel volatility = underlyingMarketData.GetVolatility();
            List<double[]> paths = new();
            for (int ω = 0; ω < drawings; ω++) {
                double[] path = new double[steps];
                path[0] = spot;
                double[] dW = noises.paths[underlying][ω];
                for (int step = 1; step < steps; step++) {
                    DateTime t = configuration.TimeDiscretization[step];
                    DateTime t_1 = configuration.TimeDiscretization[step - 1];
                    double μ = GetForwardRate(configuration.MarketData.GetDiscounter(Currencies.USD), t_1, t);
                    double b = underlyingMarketData.GetDividend() + underlyingMarketData.GetRepo();
                    double timeToMaturity = (T - t).TotalDays / 365.0;
                    double σ = volatility.getVolatility(path[step - 1], timeToMaturity);
                    double dt = (t - t_1).TotalDays / 365.0;
                    path[step] = new LogEulerScheme().Evolve(path[step - 1], timeToMaturity, dt, dW[step], new StochasticDifferentialEquationDefinition(
                        (s, t) => (μ - b) * s,
                        (s, t) => σ * s
                    ));
                }
                paths.Add(path);
            }
            return new Realizations { Paths = paths };
        }
        private static double GetForwardRate(IDiscounter discounter, DateTime from, DateTime to) {
            double dt = (to - from).TotalDays / 365.0;
            return Math.Log(discounter.GetDiscountFactor(from, to)) / dt;
        }
    }

}
