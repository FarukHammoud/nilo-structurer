using Domain;
namespace Application {
    // Hull-White model: dr(t) = [θ(t) - κr(t)]dt + σdW(t)
    public class HullWhiteDynamics : IProcessDynamics {
        private readonly double         _kappa; // mean reversion speed
        private readonly double         _sigma; // volatility
        private readonly Func<double, double> _theta;  // long term mean: time-dependent: calibrated to initial curve

        public HullWhiteDynamics(double kappa, double sigma,
                                  Func<double, double> theta) {
            _kappa = kappa;
            _sigma = sigma;
            _theta = theta;          // θ(t) fitted to discount curve
        }

        public StochasticDifferentialEquation GetSDE(double r, DateTime t_1, DateTime t) {
            double tYear = (t).Year;
            return new StochasticDifferentialEquation(
                Drift: (r, t) => _theta(tYear) - _kappa * r,  // θ(t) replaces κθ
                Diffusion: (r, t) => _sigma
            );
        }
    }
}
