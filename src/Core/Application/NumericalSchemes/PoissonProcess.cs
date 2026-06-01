using Domain;
using MathNet.Numerics.Distributions;

namespace Application {
    // Reference paper: Option Pricing when underlying stock returns are discontinous
    // Robert C. Merton, April 1975. MIT. 
    // Using the lognormal intensity subcase
    public class PoissonProcess : IJumpProcess {
        private readonly JumpParameters _parameters;

        public PoissonProcess(JumpParameters parameters) {
            _parameters = parameters;
        }

        public double Sample(double dt, Func<double> uniform) {
            int nJumps = PoissonFromUniform(_parameters.λ, dt, uniform());
            double logJump = 0;
            for (int k = 0; k < nJumps; k++) {
                double zk = Normal.InvCDF(0, 1, uniform());
                logJump += _parameters.μJ + _parameters.σJ * zk;
            }
            return logJump;
        }

        private static int SamplePoisson(double λ, double dt, Random random) {
            // Knuth algorithm — fine for small λ·dt
            double L = Math.Exp(-λ * dt);
            double p = 1.0;
            int k = 0;
            do { p *= random.NextDouble(); k++; } while (p > L);
            return k - 1;
        }

        private static int PoissonFromUniform(double λ, double dt, double u) {
            // Inverse CDF for Poisson — exact, single uniform
            double cumulative = Math.Exp(-λ * dt); // P(N=0)
            if (u < cumulative) {
                return 0;
            }
            cumulative += λ * dt * Math.Exp(-λ * dt); // P(N≤1)
            if (u < cumulative) {
                return 1;
            }
            return 2; // P(N≥2) negligible for small λ·dt, handle if needed
        }

        public double GetDrift() {
            double λ = _parameters.λ;
            double μJ = _parameters.μJ;
            double σJ = _parameters.σJ;
            double κ = Math.Exp(μJ + 0.5 * σJ * σJ) - 1;
            return λ * κ;
        }
    }
}
