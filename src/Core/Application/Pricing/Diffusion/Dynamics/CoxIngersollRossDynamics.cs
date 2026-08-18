using Domain;
namespace Application {
    // CIR model: dr(t) = κ(θ(t) - r(t))dt + σ√r(t)dW(t)
    public class CoxIngersollRossDynamics : IProcessDynamics {
        private readonly double κ; // mean reversion speed
        private readonly double σ; // volatility
        private readonly Func<double, double>  θ; // long term mean: time-dependent: calibrated to initial curve

        public CoxIngersollRossDynamics(double kappa, double sigma, Func<double, double> theta) {
            κ = kappa;
            σ = sigma;
            θ = theta;
        }

        public StochasticDifferentialEquation GetSDE(int ω, int step, DateTime t_1, DateTime t) {
            double tYear = (t).Year;
            return new StochasticDifferentialEquation(
                Drift: (r, t) => κ * (θ(tYear) - r),
                Diffusion: (r, t) => σ * Math.Sqrt(Math.Max(r, 0.0))
            );
        }
    }
}