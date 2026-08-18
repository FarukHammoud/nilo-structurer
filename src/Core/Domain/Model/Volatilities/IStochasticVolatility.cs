namespace Domain {
    /// <summary>
    /// Represents a model for stochastic volatility, 
    /// </summary>
    public interface IStochasticVolatility : IVolatility {
        double GetVolatility(int ω, int step);
    }
}
