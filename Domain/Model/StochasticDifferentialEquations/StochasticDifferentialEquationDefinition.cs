namespace Domain {
    // This record defines the structure for a stochastic differential equation (SDE) used in financial modeling.
    // dS = a(S, t) dt + b(S, t) dW
    // Diffusion Jumps
    // dS = (μ - λk)*S dt + σ*S dW + (J - 1)*S dN_t
    // λ = intensity - jumps per year
    // k = E[J-1] expected jump size
    // dN_t poisson increment, 1 with prob. λ.dt
    // J = jump size - e^(μJ + σJ Z)

    public sealed record StochasticDifferentialEquationDefinition(
        Func<double, double, double> Drift,      // (S, t) → a(S,t)
        Func<double, double, double> Diffusion); // (S, t) → b(S,t)
        
}
