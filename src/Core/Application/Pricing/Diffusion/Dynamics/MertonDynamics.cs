using Domain;

namespace Application {
    public class MertonDynamics : LevyProcessDynamics {
        public MertonDynamics(JumpParameters jumpParams) : base((t_1, t) => 0, new ConstantLocalVolatilityModel(0), jumpParams) { }
    }
}
