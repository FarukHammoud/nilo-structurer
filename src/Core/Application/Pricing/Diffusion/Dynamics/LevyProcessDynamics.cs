using Domain;

namespace Application {
    public class LevyProcessDynamics : IProcessDynamics {
        private readonly Func<DateTime, DateTime, double> _drift;
        private readonly ILocalVolatilityModel _volatility;
        private readonly IJumpProcess? _jumps;

        public double SampleJumpMultiplier(double dt, Func<double> uniform)
            => _jumps != null ? Math.Exp(_jumps.Sample(dt, uniform)) : 1.0;

        public LevyProcessDynamics(Func<DateTime, DateTime, double> drift, ILocalVolatilityModel volatility, JumpParameters? jumpParams = null) {
            _volatility = volatility;
            _jumps = jumpParams != null ? new PoissonProcess(jumpParams) : null;
            _drift = (t_1, t) => drift(t_1, t) - (_jumps?.GetDrift() ?? 0.0);
        }

        public StochasticDifferentialEquation GetSDE(double spot, DateTime t_1, DateTime t) {
            double σ = _volatility.getVolatility(spot, t.Year);
            double μ = _drift(t_1, t);
            return new StochasticDifferentialEquation(
                Drift: (s, t) => μ * s,
                Diffusion: (s, t) => σ * s
            );
        }
    }
}
