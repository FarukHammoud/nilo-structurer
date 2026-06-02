using Application;
using Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure {
    public class JsonSpotProvider : ISpotProvider {
        private readonly string _path;

        private static readonly JsonSerializerOptions JsonOptions = new() {
            PropertyNameCaseInsensitive = true,
        };

        public JsonSpotProvider(string path) {
            _path = path;
        }

        private sealed record SpotEntry(
            [property: JsonPropertyName("spot")] double Spot
        );

        public async Task<Dictionary<Underlying, double>> GetSpotsAsync(
                IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", ".."));
            string path = Path.Combine(projectRoot, _path);
            var json = await File.ReadAllTextAsync(path, ct);
            return await GetSpotsAsyncFromJson(json, underlyings, ct);
        }

        public static async Task<Dictionary<Underlying, double>> GetSpotsAsyncFromJson(
                String json, IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            Dictionary<string, SpotEntry> jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, SpotEntry>>(json, JsonOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize spots from JSON");

            Dictionary<Underlying, double> spots = new();
            foreach (Underlying underlying in underlyings) {
                if (jsonDictionary.ContainsKey(underlying.Code)) {
                    var entry = jsonDictionary[underlying.Code];
                    spots[underlying] = entry.Spot;
                }
            }
            return spots;
        }
    }
}
