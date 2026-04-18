namespace Domain {
    public interface IMarketData : IDiscounter {
        List<Underlying> GetUnderlyings();
        double GetSpot(Underlying underlying);
        double GetDrift(Underlying underlying); 
        ILocalVolatilityModel GetVolatility(Underlying underlying);
        double[,] GetCorrelationMatrix(List<Underlying> underlyings);
    }
}
