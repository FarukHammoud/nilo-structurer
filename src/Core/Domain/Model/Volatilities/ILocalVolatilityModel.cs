namespace Domain {
    public interface ILocalVolatilityModel : IVolatility {
        double getVolatility(double spot, double timeToMaturity);
    }
}
