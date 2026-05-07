namespace Domain {
    public interface IEquityMarketData : IUnderlyingMarketData {
        double GetDividend();
        double GetRepo();
    }
}
