using Domain;

namespace Application {
    public interface IRateCurveProvider {
        Task<Dictionary<Currency, Curve>> GetRateCurveAsync(
                IEnumerable<Currency> currencies, CancellationToken ct = default);
    }
}
