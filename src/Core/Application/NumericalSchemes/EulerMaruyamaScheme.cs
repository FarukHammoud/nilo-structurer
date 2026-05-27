using Domain;

namespace Application {
    public sealed class EulerMaruyamaScheme : INumericalScheme {
        public double Evolve(double S, double t, double dt, double dW, StochasticDifferentialEquationDefinition sde)
            => S + sde.Drift(S, t) * dt + sde.Diffusion(S, t) * Math.Sqrt(dt) * dW;
    }
}
