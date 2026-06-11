namespace Domain {
    // a crime against humanity, only the time to decide the api to register dynamics
    public class ProcessDynamicsMarketData : IProcessDynamics, IUnderlyingMarketData {
        private readonly IProcessDynamics _dynamics;
        public ProcessDynamicsMarketData(IProcessDynamics dynamics) {
            _dynamics = dynamics;
        }

        public StochasticDifferentialEquation GetSDE(double state, DateTime t_1, DateTime t) {
            return _dynamics.GetSDE(state, t_1, t);
        }

        public double SampleJumpMultiplier(double dt, Func<double> uniform) {
            return _dynamics.SampleJumpMultiplier(dt, uniform);
        }

        public double GetSpot() {
            throw new NotImplementedException();
        }

        public double GetCarry() {
            throw new NotImplementedException();
        }

        public ILocalVolatilityModel GetVolatility() {
            throw new NotImplementedException();
        }

    }
}
