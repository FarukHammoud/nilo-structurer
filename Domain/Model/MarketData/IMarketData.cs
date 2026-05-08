namespace Domain {
    public interface IMarketData : IFxConverter {
        IList<Underlying> Underlyings { get; }
        IList<Currency> Currencies { get; }
        IDiscounter GetDiscounter(Currency currency);
        double[,] GetCorrelationMatrix(IList<Underlying> underlyings);
        IUnderlyingMarketData GetUnderlyingMarketData(Underlying underlying);
    }
}
