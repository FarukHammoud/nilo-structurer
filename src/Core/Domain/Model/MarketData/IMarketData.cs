namespace Domain {
    public interface IMarketData : IFxConverter {
        IList<Underlying> Underlyings { get; }
        IList<Currency> Currencies { get; }
        IDiscounter GetDiscounter(Currency currency);
        double GetCorrelation(Underlying underlying1, Underlying underlying2);
        IUnderlyingMarketData GetUnderlyingMarketData(Underlying underlying);
    }
}
