namespace Domain {
    /// <summary>
    /// Represents a model for local volatility, which is a function of both the underlying asset's price (spot) and time to maturity. 
    /// </summary>
    public interface ILocalVolatilityModel : IVolatility {
        double GetVolatility(double spot, double timeToMaturity);
    }
}
