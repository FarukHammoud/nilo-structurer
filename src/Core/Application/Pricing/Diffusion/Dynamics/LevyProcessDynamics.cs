using Domain;

namespace Application {
    public class LevyProcessDynamics : IProcessDynamics {
        private Func<DateTime, DateTime, double> _drift;
        private IVolatility _volatility;
        private IJumpProcess? _jumps;

        public double SampleJumpMultiplier(double dt, Func<double> uniform)
            => _jumps != null ? Math.Exp(_jumps.Sample(dt, uniform)) : 1.0;

        public LevyProcessDynamics SetDrift(Func<DateTime, DateTime, double> drift) {
            _drift = drift;
            return this;
        }

        public LevyProcessDynamics SetVolatility(IVolatility volatility) {
            _volatility = volatility;
            return this;
        }

        public LevyProcessDynamics SetJumps(JumpParameters jumpParameters) {
            _jumps = new PoissonProcess(jumpParameters);
            return this;
        }

        public StochasticDifferentialEquation GetSDE(int ω, int step, DateTime t_1, DateTime t) {
            double μ = _drift(t_1, t) - (_jumps?.GetDrift() ?? 0);
            if (_volatility is ILocalVolatilityModel localVolatility) {
                return new StochasticDifferentialEquation(
                    Drift: (s, t) => μ * s,
                    Diffusion: (s, t) => localVolatility.GetVolatility(s, t) * s
                );
            } else if (_volatility is IStochasticVolatility stochasticVolatility) {
                return new StochasticDifferentialEquation(
                    Drift: (s, t) => μ * s,
                    Diffusion: (s, t) => stochasticVolatility.GetVolatility(ω, step) * s
                );
            } else {
                throw new InvalidOperationException("Unsupported volatility model.");
            }
        }
    }
}
