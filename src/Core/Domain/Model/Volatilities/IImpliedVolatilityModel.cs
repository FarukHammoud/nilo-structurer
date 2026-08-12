namespace Domain {
    /// <summary>
    /// Represents a model for implied volatility, which is simply the volatility that makes the theoretical black-scholes price of an option equal to its market price.
    /// </summary>
    public interface IImpliedVolatilityModel : IVolatility {
        double GetVolatility(double strike, DateTime time);
    }
}
