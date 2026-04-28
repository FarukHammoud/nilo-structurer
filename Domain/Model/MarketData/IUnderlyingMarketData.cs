namespace Domain {
    public interface IUnderlyingMarketData {
        double GetSpot();
        ILocalVolatilityModel GetVolatility();
        double GetDividend();
        double GetRepo();
    }
}
