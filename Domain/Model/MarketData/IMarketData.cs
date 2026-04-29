namespace Domain {
    public interface IMarketData : IDiscounter {
        List<Underlying> GetUnderlyings();
        double[,] GetCorrelationMatrix(List<Underlying> underlyings);
        IUnderlyingMarketData GetUnderlyingMarketData(Underlying underlying);
        double GetFxRate(Currency from, Currency to);
    }
}
