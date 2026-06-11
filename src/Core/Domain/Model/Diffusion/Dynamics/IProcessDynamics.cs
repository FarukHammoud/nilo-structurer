namespace Domain {
    public interface IProcessDynamics {
        StochasticDifferentialEquation GetSDE(double state, DateTime t_1, DateTime t);
        double SampleJumpMultiplier(double dt, Func<double> uniform) {
            return 1.0;
        }
    }
}
