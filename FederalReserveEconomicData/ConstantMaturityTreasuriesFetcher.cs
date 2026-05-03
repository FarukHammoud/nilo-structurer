using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FederalReserveEconomicData {
    public class ConstantMaturityTreasuriesFetcher {
        private readonly FredApiKey _apiKey;
        public ConstantMaturityTreasuriesFetcher(IOptions<FredApiKey> apiKey) {
            _apiKey = apiKey.Value;
        }

        private async Task<Dictionary<double, double>> Fetch() {
            string[] series = ["DGS1MO", "DGS3MO", "DGS6MO", "DGS1", "DGS2", "DGS5", "DGS10", "DGS30"];
            double[] tenors = [1 / 12.0, 0.25, 0.5, 1, 2, 5, 10, 30];

            var rates = new Dictionary<double, double>();
            using HttpClient client = new();

            for (int i = 0; i < series.Length; i++) {
                string url = $"https://api.stlouisfed.org/fred/series/observations" +
                             $"?series_id={series[i]}&api_key={_apiKey.Key}&sort_order=desc&limit=1&file_type=json";
                var json = await client.GetStringAsync(url);
                var doc = JsonDocument.Parse(json);
                double rate = double.Parse(doc.RootElement
                    .GetProperty("observations")[0]
                    .GetProperty("value").GetString()!) / 100.0;
                rates[tenors[i]] = rate;
            }
            return rates;
        }

        public async Task FetchAndDump() {
            Dictionary<double, double> rates = await Fetch();
            var fredRates = rates.Select((kv, i) => new FredRate(kv.Key, kv.Value, DateTime.Today));
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string dataFolder = Path.Combine(projectRoot, "Data");
            Directory.CreateDirectory(dataFolder);
            string path = Path.Combine(dataFolder, "fred_rates.csv");
            FredDump.Write(path, fredRates);
        }
    }
}
