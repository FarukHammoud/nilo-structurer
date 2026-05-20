using Application;
using Domain;

namespace Infrastructure {
    public class YahooSpotProvider : ISpotProvider {
        public async Task<Dictionary<Underlying, double>> GetSpotsAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            throw new NotImplementedException();
        }

    }
}
