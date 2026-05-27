using Domain;

namespace Application{
    public interface ISpotProvider {
        Task<Dictionary<Underlying, double>> GetSpotsAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default);
    }
}
