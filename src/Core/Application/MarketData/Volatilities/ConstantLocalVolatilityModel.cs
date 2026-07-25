using Domain;

namespace Application {
    public class ConstantLocalVolatilityModel : ILocalVolatilityModel {
        private double volatility;
        public ConstantLocalVolatilityModel(double volatility) {
            this.volatility = volatility;
        }
        public double GetVolatility(double spot, double timeToMaturity) {
            return volatility;
        }
    }
}
