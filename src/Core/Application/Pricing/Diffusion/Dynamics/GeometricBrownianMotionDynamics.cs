using Domain;

namespace Application {
    public class GeometricBrownianMotionDynamics : LevyProcessDynamics {
        public GeometricBrownianMotionDynamics(double mu, double carry, ILocalVolatilityModel vol) : base((t_1, t) => mu - carry, vol) {
        }
    }
}
