namespace Domain {
    public interface ILocalVolatilityModel : IVolatility {
        double GetVolatility(double spot, double timeToMaturity);
    }
}
