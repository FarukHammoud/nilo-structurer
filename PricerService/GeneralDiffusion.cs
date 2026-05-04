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
            IMarketData marketData = configuration.MarketData;
            IDiscounter discounter = marketData.GetDiscounter(underlying.Currency);
            IUnderlyingMarketData underlyingMarketData = marketData.GetUnderlyingMarketData(underlying);
            double spot = underlyingMarketData.GetSpot();
            DateTime T = configuration.TimeDiscretization.LastOrDefault();
            ILocalVolatilityModel volatility = underlyingMarketData.GetVolatility();
            double μ_adjustment = 0;
            IJumpProcess? jumpProcess = null;
            if (configuration.JumpParameters != null) {
                jumpProcess = new PoissonProcess(configuration.JumpParameters);
                double λ = configuration.JumpParameters.λ;
                double μJ = configuration.JumpParameters.μJ;
                double σJ = configuration.JumpParameters.σJ;
                double κ = Math.Exp(μJ + 0.5 * σJ * σJ) - 1;
                μ_adjustment = -λ * κ; // adjust drift to keep martingale property
            }
            List<double[]> paths = new();
            for (int ω = 0; ω < drawings; ω++) {
                double[] path = new double[steps];
                path[0] = spot;
                double[] dW = noises.paths[underlying][ω];
                for (int step = 1; step < steps; step++) {
                    DateTime t = configuration.TimeDiscretization[step];
                    DateTime t_1 = configuration.TimeDiscretization[step - 1];
                    double μ = GetForwardRate(discounter, t_1, t);
                    if (underlying is CurrencyPair fxPair) {
                        // For FX pairs: drift = r_base - r_quote (interest rate parity)
                        IDiscounter baseDiscounter = marketData.GetDiscounter(fxPair.Base);
                        IDiscounter quoteDiscounter = marketData.GetDiscounter(fxPair.Quote);
                        double r_base = GetForwardRate(baseDiscounter, t_1, t);
                        double r_quote = GetForwardRate(quoteDiscounter, t_1, t);
                        μ = r_base - r_quote;
                    } 
                    μ += μ_adjustment;
                    double b = underlyingMarketData.GetDividend() + underlyingMarketData.GetRepo();
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
        private static double GetForwardRate(IDiscounter discounter, DateTime from, DateTime to) {
            double dt = (to - from).TotalYears;
            return Math.Log(discounter.GetDiscountFactor(from, to)) / dt;
        }
    }

}
