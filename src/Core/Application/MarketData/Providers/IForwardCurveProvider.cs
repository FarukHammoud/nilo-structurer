using Domain;

namespace Application {
    public interface IForwardCurveProvider {
        Task<Dictionary<Underlying, Curve>> GetForwardCurveAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default);
    }
}
