using Domain;

namespace Application {
    public class MertonJumpModel : ConstantVolatilityModel, IMertonJumpModel {
        public JumpParameters JumpParameters { get; private set; }

        public MertonJumpModel(JumpParameters jumpParameters, double volatility) : base(volatility) {
            JumpParameters = jumpParameters;
        }
    }
}
