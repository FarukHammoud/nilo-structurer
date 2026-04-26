namespace Domain {
    // This record defines the structure for a stochastic differential equation (SDE) used in financial modeling.
    // dS = a(S, t) dt + b(S, t) dW
    public sealed record StochasticDifferentialEquationDefinition (
        Func<double, double, double> Drift,      // (S, t) → a(S,t)
        Func<double, double, double> Diffusion); // (S, t) → b(S,t)
}
