namespace Domain {
    public interface IMarketData : IFxConverter {
        List<Underlying> GetUnderlyings();
        IDiscounter GetDiscounter(Currency currency);
        double[,] GetCorrelationMatrix(List<Underlying> underlyings);
        IUnderlyingMarketData GetUnderlyingMarketData(Underlying underlying);
    }
}
