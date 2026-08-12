using Domain;

namespace Application {
    public class GeneralDiffusion {

        private static readonly IDayCountConvention _dayCountConvention = new Actual365();
        public static Diffusion DiffuseMultiUnderlying(IDiffusionConfiguration configuration) {
            BrowniansResult noises = new BrowniansService()
                .CreateCorrelatedBrownians(configuration);
            Diffusion diffusion = new(configuration.TimeDiscretization);
            foreach (Underlying underlying in configuration.Underlyings) {
                diffusion[underlying] = DiffuseUnderlying(configuration, underlying, noises);
            }
            return diffusion;
        }

        private static Realizations DiffuseUnderlying(IDiffusionConfiguration configuration, Underlying underlying, BrowniansResult noises) {
            int steps    = configuration.TimeDiscretization.Count;
            int drawings = configuration.NumberOfDrawings;
            IMarketData marketData               = configuration.MarketData;
            INumericalScheme scheme              = configuration.NumericalScheme;
            IUnderlyingMarketData underlyingData = marketData.GetUnderlyingMarketData(underlying);
            IDriftProvider driftProvider         = new DriftProvider();

            // hack, we should get dynamics from somewhere (market data?)
            IProcessDynamics dynamics;
            double spot = underlyingData.GetSpot();
            if (underlying is ShortRate shortRate) {
                dynamics = marketData.GetShortRateDynamics(shortRate.Currency);
                scheme = new EulerMaruyamaScheme();
            } else {
                IDiscounter discounter = marketData.GetDiscounter(configuration.Currency);
                IImpliedVolatilityModel impliedVolatility = underlyingData.GetVolatility();
                ILocalVolatilityModel volatility = new DupireLocalVolatilityModel(impliedVolatility, discounter, spot);
                JumpParameters? jumpParameters = null;
                if (impliedVolatility is MertonJumpModel merton) {
                    volatility = merton;
                    jumpParameters = merton.JumpParameters;
                } 
                Func<DateTime, DateTime, double> drift = (t_1, t) => driftProvider.GetDrift(underlying, configuration.Currency, marketData, t_1, t);
                double carry                           = underlyingData.GetCarry();
                dynamics = new LevyProcessDynamics((t_1, t) => drift(t_1, t) - carry, volatility, jumpParameters);
            }
            DateTime T  = configuration.TimeDiscretization.LastOrDefault();

            Realizations realizations = new();
            Random jumpRandom = new Random();
            for (int ω = 0; ω < drawings; ω++) {
                SimulatedPath path = new(steps);
                SimulatedPath dW   = noises.Paths[underlying][ω];
                path[0]            = spot;
                for (int step = 1; step < steps; step++) {
                    DateTime t   = configuration.TimeDiscretization[step];
                    DateTime t_1 = configuration.TimeDiscretization[step - 1];
                    double dt    = _dayCountConvention.YearFraction(t_1, t);

                    StochasticDifferentialEquation sde = dynamics.GetSDE(path[step - 1], t_1, t);
                    path[step]  = scheme.Evolve(path[step - 1], t, dt, dW[step], sde);
                    path[step] *= dynamics.SampleJumpMultiplier(dt, jumpRandom.NextDouble);
                }
                realizations.AddPath(path);
            }
            return realizations;
        }
    }
}
