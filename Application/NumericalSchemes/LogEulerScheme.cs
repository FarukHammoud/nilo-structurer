using Domain;

namespace Application {
    public sealed class LogEulerScheme : INumericalScheme {
        public double Evolve(double S, double t, double dt, double dW, StochasticDifferentialEquationDefinition sde) {
            // Convert absolute drift/diffusion to relative (per unit S)
            double μ = sde.Drift(S, t) / S;
            double σ = sde.Diffusion(S, t) / S;
            return S * Math.Exp((μ - 0.5 * σ * σ) * dt
                                 + σ * Math.Sqrt(dt) * dW);
        }
    }
}
