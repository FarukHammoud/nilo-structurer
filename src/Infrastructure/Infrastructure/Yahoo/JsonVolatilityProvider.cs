using Application;
using Domain;

namespace Infrastructure {
    public class JsonVolatilityProvider : IVolatilityProvider {
        private readonly String _path;

        public JsonVolatilityProvider(String path) {
            _path = path;
        }

        public async Task<Dictionary<Underlying, ILocalVolatilityModel>> GetVolatilitiesAsync(IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            // PLACEHOLDER: In a real implementation, you would read the JSON file at _path, parse it, and create appropriate ILocalVolatilityModel instances based on the data. For this example, we will return a constant local volatility model for each underlying.
            return underlyings.ToDictionary(
                underlying => underlying, 
                underlying => (ILocalVolatilityModel) new ConstantLocalVolatilityModel(0.2));
        }
    }
}
