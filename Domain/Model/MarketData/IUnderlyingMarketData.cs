namespace Domain {
    public interface IUnderlyingMarketData {

        double GetSpot();
        ILocalVolatilityModel GetVolatility();
        double GetCarry(); // This can be used to represent dividends, repo rates, foreign currency rates, etc.
    }
}
