namespace Domain {
    /// <summary>
    /// Represents the parameters for a jump process in a stochastic differential equation.
    /// dS = (μ - λk)*S dt + σ*S dW + (J - 1)*S dN_t
    /// λ = intensity - jumps per year
    /// k = E[J-1] expected jump size
    /// dN_t poisson increment, 1 with prob. λ.dt
    /// J = jump size - e^(μJ + σJ Z)
    /// </summary>
    /// <param name="λ">Intensity [jumps per year]</param>
    /// <param name="μJ">Expected jump size</param>
    /// <param name="σJ">Jump size volatility</param>
    public record JumpParameters(double λ, double μJ, double σJ);
}
