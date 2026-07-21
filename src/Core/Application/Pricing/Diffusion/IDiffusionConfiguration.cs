using Domain;
namespace Application {
    public interface IDiffusionConfiguration : IPricerConfiguration, IBrowniansConfiguration {
        IMarketData MarketData { get; }
        IList<DateTime> TimeDiscretization { get; }
        Currency Currency { get; }
        bool WithControlVariate { get; }
        bool HasStochasticRate { get; }
        INumericalScheme NumericalScheme { get; } 
    }
}
