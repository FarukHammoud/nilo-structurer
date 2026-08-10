using Domain;

namespace Application {
    public class ConstantVolatilityModel : ILocalVolatilityModel, IImpliedVolatilityModel {
        private double volatility;
        public ConstantVolatilityModel(double volatility) {
            this.volatility = volatility;
        }
        public double GetVolatility(double spot, double timeToMaturity) {
            return volatility;
        }
    }
}
