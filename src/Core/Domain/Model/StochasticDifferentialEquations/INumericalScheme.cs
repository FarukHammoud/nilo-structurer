namespace Domain {
    public interface INumericalScheme {
        double Evolve(double S, double t, double dt, double dW, StochasticDifferentialEquation sde);
    }
}
