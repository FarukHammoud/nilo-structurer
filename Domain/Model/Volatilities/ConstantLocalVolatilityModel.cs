namespace Domain {
    public class ConstantLocalVolatilityModel : ILocalVolatilityModel {
        private double volatility;
        public ConstantLocalVolatilityModel(double volatility) {
            this.volatility = volatility;
        }
        public double getVolatility(double spot, double timeToMaturity) {
            return volatility;
        }
    }
}
