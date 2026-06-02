using Application;
using Domain;
using System.Diagnostics;
using System.Text.Json;

namespace Infrastructure {
    // Checks the cache, then calls a Python script to fetch missing spots, and finally merges results.
    public class YahooSpotProvider : ISpotProvider {
        private readonly string _cacheJsonPath;
        private readonly string SPOT_SCRIPT = "dump_spots.py";
        private readonly TimeSpan _cacheTtl; // not yet implemented

        public YahooSpotProvider(
        string cacheJsonPath,
        TimeSpan? cacheTtl = null) {
            _cacheJsonPath = cacheJsonPath;
            _cacheTtl = cacheTtl ?? TimeSpan.FromDays(1);
        }

        public async Task<Dictionary<Underlying, double>> GetSpotsAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            Dictionary<Underlying, double> cachedSpots = await LoadCachedSpotsAsync(underlyings, ct);
            IEnumerable<Underlying> missingUnderlyings = underlyings.Where(u => !cachedSpots.ContainsKey(u));
            if (!missingUnderlyings.Any()) {
                return cachedSpots;
            }
            Dictionary<Underlying, double> missingSpots = await FetchFromPythonAsync(missingUnderlyings, ct);
            return cachedSpots.Concat(missingSpots).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public async Task<Dictionary<Underlying, double>> LoadCachedSpotsAsync(IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            JsonSpotProvider jsonProvider = new JsonSpotProvider(_cacheJsonPath);
            return await jsonProvider.GetSpotsAsync(underlyings, ct);
        }


        private async Task<Dictionary<Underlying, double>> FetchFromPythonAsync(IEnumerable<Underlying> underlyings, CancellationToken ct) {
            var tickers = underlyings.Select(u => u.Code);
      
            var scriptPath = Path.Combine("yahoo", "scripts", SPOT_SCRIPT);
            var args = string.Join(" ", tickers.Select(ticker => $"\"{ticker}\""));

            String json = await new PythonExecuter(scriptPath).Run(args, ct);
            return await JsonSpotProvider.GetSpotsAsyncFromJson(json, underlyings, ct);
        }
    }
}
