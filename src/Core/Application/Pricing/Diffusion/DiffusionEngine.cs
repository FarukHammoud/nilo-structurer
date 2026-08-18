using Domain;

namespace Application {
    public class DiffusionEngine : IDiffusionEngine {

        private IDayCountConvention _dayCountConvention;
        private IDriftProvider _driftProvider;
        private ISchemeProvider _schemeProvider;

        public DiffusionEngine() : this(
            dayCountConvention: new Actual365(),
            driftProvider: new DriftProvider(),
            schemeProvider: new SchemeProvider()) { }

        public DiffusionEngine(IDayCountConvention dayCountConvention, IDriftProvider driftProvider, ISchemeProvider schemeProvider) {
            _dayCountConvention = dayCountConvention;
            _driftProvider = driftProvider;
            _schemeProvider = schemeProvider;
        }
      
        public Diffusion Diffuse(IDiffusionConfiguration configuration) {
            BrowniansResult noises = new BrowniansService()
                .CreateCorrelatedBrownians(configuration);
            Diffusion diffusion = new(configuration.TimeDiscretization);
            var ordered = configuration.Underlyings
                .OrderBy(u => u is InstantaneousVolatility ? 0 : 1);
            foreach (Underlying underlying in ordered) {
                diffusion[underlying] = DiffuseUnderlying(configuration, underlying, noises, diffusion);
            }
            return diffusion;
        }

        private Realizations DiffuseUnderlying(IDiffusionConfiguration configuration, Underlying underlying, BrowniansResult noises, Diffusion diffusion) {
            int steps    = configuration.TimeDiscretization.Count;
            int drawings = configuration.NumberOfDrawings;
            IMarketData           marketData     = configuration.MarketData;
            Currency              currency       = configuration.Currency;
            IUnderlyingMarketData underlyingData = marketData.GetUnderlyingMarketData(underlying);
            
            IProcessDynamics dynamics = marketData.GetDynamics(underlying);
            INumericalScheme scheme   = configuration.NumericalSchemeOverride ??
                _schemeProvider.GetScheme(dynamics);

            double spot = underlyingData.GetSpot();
            // TODO: Needs to be completed on market data side
            if (dynamics is LevyProcessDynamics levyDynamics) {
                Func<DateTime, DateTime, double> drift = _driftProvider.GetDrift(underlying, currency, marketData);
                double carry                           = underlyingData.GetCarry();
                levyDynamics.SetDrift((t_1, t) => drift(t_1, t) - carry);
                    
                IImpliedVolatilityModel impliedVolatility = underlyingData.GetVolatility();
                if (impliedVolatility is MertonJumpModel merton) {
                    levyDynamics.SetVolatility(merton);
                    levyDynamics.SetJumps(merton.JumpParameters);
                } else if (underlying is Equity equity && marketData.Underlyings.Contains(new InstantaneousVolatility(equity))) {
                    Realizations volatilityRealizations = diffusion[new InstantaneousVolatility(equity)];
                    IStochasticVolatility stochasticVolatility = new StochasticVolatilityModel(volatilityRealizations);
                    levyDynamics.SetVolatility(stochasticVolatility);
                } else { 
                    IDiscounter discounter                = marketData.GetDiscounter(currency);
                    ILocalVolatilityModel localVolatility = new DupireLocalVolatilityModel(impliedVolatility, discounter, spot);
                    levyDynamics.SetVolatility(localVolatility);
                }
            }

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

                    StochasticDifferentialEquation sde = dynamics.GetSDE(ω, step - 1, t_1, t);
                    path[step]  = scheme.Evolve(path[step - 1], t, dt, dW[step], sde);
                    path[step] *= dynamics.SampleJumpMultiplier(dt, jumpRandom.NextDouble);
                }
                realizations.AddPath(path);
            }
            return realizations;
        }
    }
}
