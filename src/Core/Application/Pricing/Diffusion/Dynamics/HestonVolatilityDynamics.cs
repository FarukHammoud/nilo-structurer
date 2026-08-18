using Domain;

namespace Application {
    public class HestonVolatilityDynamics : IProcessDynamics {
        private readonly double λ; // speed of reversion
        private readonly double v_; // long-term mean
        private readonly double η; // volatility of volatility
        public HestonVolatilityDynamics(double λ, double v_, double η) {
            this.λ = λ;
            this.v_ = v_;
            this.η = η;
        }

        public StochasticDifferentialEquation GetSDE(int ω, int step, DateTime t_1, DateTime t) {
            return new StochasticDifferentialEquation(
                Drift: (v, t) => -λ * (v_ - v),
                Diffusion: (v, t) => η * Math.Sqrt(Math.Max(0, v))   
            );
        }
    }
}
