namespace Domain {
    // a crime against humanity, only the time to decide the api to register dynamics
    public class ProcessDynamicsMarketData : IProcessDynamics, IUnderlyingMarketData {
        private readonly IProcessDynamics _dynamics;
        private readonly double _spotRate;
        public ProcessDynamicsMarketData(IProcessDynamics dynamics, double spotRate) {
            _dynamics = dynamics;
            _spotRate = spotRate;
        }

        public StochasticDifferentialEquation GetSDE(double state, DateTime t_1, DateTime t) {
            return _dynamics.GetSDE(state, t_1, t);
        }

        public double SampleJumpMultiplier(double dt, Func<double> uniform) {
            return _dynamics.SampleJumpMultiplier(dt, uniform);
        }

        public double GetSpot() {
            return _spotRate;
        }

        public double GetCarry() {
            return 0;
        }

        public ILocalVolatilityModel GetVolatility() {
            throw new NotImplementedException();
        }

    }
}
