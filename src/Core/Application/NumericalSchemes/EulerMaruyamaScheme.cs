using Domain;
using System.Diagnostics;

namespace Application {
    public sealed class EulerMaruyamaScheme : INumericalScheme {
        public bool EnsurePositivity { get; set; } = false;
        public double Evolve(double S, DateTime t, double dt, double dW, StochasticDifferentialEquation sde) {
            double nextValue = S + sde.Drift(S, t) * dt + sde.Diffusion(S, t) * Math.Sqrt(dt) * dW;
            if (EnsurePositivity && nextValue < 0) {
                Debug.WriteLine("The next value is negative, which violates the positivity constraint.");
                nextValue = 0;
            }
            return nextValue;
        }
    }
}
