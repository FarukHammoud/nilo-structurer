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
            double spot = underlyingMarketData.GetSpot();
            DateTime T = configuration.TimeDiscretization.LastOrDefault();
            ILocalVolatilityModel volatility = underlyingMarketData.GetVolatility();
            double μ_adjustment = 0;
            IJumpProcess? jumpProcess = configuration.JumpParameters != null ? new PoissonProcess(configuration.JumpParameters) : null;
			if (jumpProcess != null) {
				μ_adjustment -= jumpProcess.GetDrift();
			}
			List<double[]> paths = new();
            for (int ω = 0; ω < drawings; ω++) {
                double[] path = new double[steps];
                path[0] = spot;
                double[] dW = noises.paths[underlying][ω];
                for (int step = 1; step < steps; step++) {
                    DateTime t = configuration.TimeDiscretization[step];
                    DateTime t_1 = configuration.TimeDiscretization[step - 1];
					double μ = GetDrift(underlying, configuration.Currency, marketData, t_1, t, μ_adjustment);
					double b = underlyingMarketData.GetCarry();
                    double timeToMaturity = (T - t).TotalYears;
                    double σ = volatility.getVolatility(path[step - 1], timeToMaturity);
                    double dt = (t - t_1).TotalYears;
                    path[step] = new LogEulerScheme().Evolve(path[step - 1], timeToMaturity, dt, dW[step], new StochasticDifferentialEquationDefinition(
                        (s, t) => (μ - b) * s,
                        (s, t) => σ * s
                    ));
                    if (jumpProcess != null) {
                        path[step] *= jumpProcess.Sample(dt, new Random().NextDouble());
                    }
                }
                paths.Add(path);
            }
            return new Realizations { Paths = paths };
        }

        private static double GetDrift(Underlying underlying, Currency diffusionCurrency, IMarketData marketData, DateTime t_1, DateTime t, double μ_adjustment) {
			double μ = 0;
            IDiscounter discounter = marketData.GetDiscounter(diffusionCurrency);
			if (underlying is Equity equity) {
				μ = discounter.GetForwardRate(t_1, t);
				if (equity.Currency != diffusionCurrency) {
					IDiscounter underlyingDiscounter = marketData.GetDiscounter(underlying.Currency);
					double r_foreign = underlyingDiscounter.GetForwardRate(t_1, t);
					μ -= r_foreign;
				}
			} else if (underlying is CurrencyPair fxPair) {
				IDiscounter baseDiscounter = marketData.GetDiscounter(fxPair.Base);
				IDiscounter quoteDiscounter = marketData.GetDiscounter(fxPair.Quote);
				double r_base = baseDiscounter.GetForwardRate(t_1, t);
				double r_quote = quoteDiscounter.GetForwardRate(t_1, t);
				μ = r_quote - r_base;
			}
			μ += μ_adjustment;
            return μ;
		}
    }
}
