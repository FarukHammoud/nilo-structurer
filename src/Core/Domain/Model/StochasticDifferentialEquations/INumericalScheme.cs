namespace Domain {
    public interface INumericalScheme {
        double Evolve(double S, DateTime t, double dt, double dW, StochasticDifferentialEquation sde);
    }
}
