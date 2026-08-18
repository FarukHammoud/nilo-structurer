namespace Domain {
    public interface IProcessDynamics {
        StochasticDifferentialEquation GetSDE(int ω, int step, DateTime t_1, DateTime t);
        double SampleJumpMultiplier(double dt, Func<double> uniform) {
            return 1.0;
        }
    }
}
