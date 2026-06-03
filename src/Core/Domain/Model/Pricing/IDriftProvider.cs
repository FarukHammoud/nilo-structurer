namespace Domain {
    public interface IDriftProvider {
        // should it live somewhere else? any underlying has to implement a local drift provider
        double GetDrift(Underlying underlying, Currency diffusionCurrency, IMarketData marketData, DateTime t_1, DateTime t);
    }
}
