using Domain;

namespace Application {
    public interface IVolatilityProvider {
        Task<Dictionary<Underlying, ILocalVolatilityModel>> GetVolatilitiesAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default);
    }
}
