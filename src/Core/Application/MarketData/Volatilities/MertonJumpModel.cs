using Domain;

namespace Application {
    public class MertonJumpModel : IMertonJumpModel, ILocalVolatilityModel {
        public JumpParameters JumpParameters { get; private set; }

        private double _volatility;
        public MertonJumpModel(JumpParameters jumpParameters, double volatility) {
            JumpParameters = jumpParameters;
            _volatility = volatility;
        }

        public double getVolatility(double spot, double timeToMaturity) {
            return _volatility;
        }
    }
}
