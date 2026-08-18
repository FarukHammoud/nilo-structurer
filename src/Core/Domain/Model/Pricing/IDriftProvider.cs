namespace Domain {
    public interface IDriftProvider {
        // TODO: should it live somewhere else? any underlying has to implement a local drift provider
        Func<DateTime, DateTime, double> GetDrift(Underlying underlying, Currency diffusionCurrency, IMarketData marketData);
    }
}
