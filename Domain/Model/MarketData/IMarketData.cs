namespace Domain {
    public interface IMarketData {
        List<Underlying> GetUnderlyings();
        double GetDiscountFactor(DateTime date, DateTime today);
        double GetSpot(Underlying underlying);
        double GetDrift(Underlying underlying); 
        ILocalVolatilityModel GetVolatility(Underlying underlying);
        double[,] GetCorrelationMatrix(List<Underlying> underlyings);
    }
}
