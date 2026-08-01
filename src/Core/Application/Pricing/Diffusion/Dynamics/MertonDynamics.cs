using Domain;

namespace Application {
    public class MertonDynamics : LevyProcessDynamics {
        public MertonDynamics(JumpParameters jumpParams) : base((t_1, t) => 0, new ConstantVolatilityModel(0), jumpParams) { }
    }
}
