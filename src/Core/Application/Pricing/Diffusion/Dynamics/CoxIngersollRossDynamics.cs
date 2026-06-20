using Domain;
namespace Application {
    // CIR model: dr(t) = κ(θ(t) - r(t))dt + σ√r(t)dW(t)
    public class CoxIngersollRossDynamics : IProcessDynamics {
        private readonly double               _kappa; // mean reversion speed
        private readonly double               _sigma; // volatility
        private readonly Func<double, double>  _theta; // long term mean: time-dependent: calibrated to initial curve

        public CoxIngersollRossDynamics(double kappa, double sigma,
                              Func<double, double> theta) {
            _kappa = kappa;
            _sigma = sigma;
            _theta = theta;
        }

        public StochasticDifferentialEquation GetSDE(double r, DateTime t_1, DateTime t) {
            double tYear = (t).Year;
            return new StochasticDifferentialEquation(
                Drift: (r, t) => _kappa * (_theta(tYear) - r),
                Diffusion: (r, t) => _sigma * Math.Sqrt(Math.Max(r, 0.0))
            );
        }
    }
}