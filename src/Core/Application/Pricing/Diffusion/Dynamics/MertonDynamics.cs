using Domain;

namespace Application {
    public class MertonDynamics : LevyProcessDynamics {
        public MertonDynamics(JumpParameters jumpParams) {
            SetDrift((t_1, t) => 0);
            SetVolatility(new ConstantVolatilityModel(0));
            SetJumps(jumpParams);
        }
    }
}
