using Domain;

namespace Application {
    public interface IOptionPriceProvider {
        Task<Dictionary<Underlying, Dictionary<VanillaContract, OptionMarketData>>> GetOptionPricesAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default);
    }
}
