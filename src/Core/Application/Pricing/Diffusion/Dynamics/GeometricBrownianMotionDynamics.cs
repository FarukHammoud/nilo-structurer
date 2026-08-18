using Domain;

namespace Application {
    public class GeometricBrownianMotionDynamics : LevyProcessDynamics {
        public GeometricBrownianMotionDynamics(double mu, double carry, ILocalVolatilityModel vol) {
            SetDrift((t_1, t) => mu - carry);
            SetVolatility(vol);
        }
    }
}
