namespace Domain {
    // Diffusion Jumps
    // dS = (μ - λk)*S dt + σ*S dW + (J - 1)*S dN_t
    // λ = intensity - jumps per year
    // k = E[J-1] expected jump size
    // dN_t poisson increment, 1 with prob. λ.dt
    // J = jump size - e^(μJ + σJ Z)
    public record JumpParameters(double λ, double μJ, double σJ);
}
