using Domain;

namespace Application {
    public interface IVolatilityProvider {
        Task<Dictionary<Underlying, IImpliedVolatilityModel>> GetVolatilitiesAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default);
    }
}
