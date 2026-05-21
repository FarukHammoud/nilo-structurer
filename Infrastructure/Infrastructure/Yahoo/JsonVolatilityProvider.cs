using Application;
using Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure {
    public class JsonVolatilityProvider : IVolatilityProvider {
        private readonly string _path;
        private readonly IVolatilitySurfaceBuilder _builder;

        private static readonly JsonSerializerOptions JsonOptions = new() {
            PropertyNameCaseInsensitive = true,
        };

        public JsonVolatilityProvider(string path, IVolatilitySurfaceBuilder builder) {
            _path = path;
            _builder = builder;
        }

        private sealed record StockOptions(
            [property: JsonPropertyName("underlyingPrice")] double UnderlyingPrice,
            [property: JsonPropertyName("asOf")] string AsOf,
            [property: JsonPropertyName("chains")] Dictionary<string, ChainData> Chains
        );

        private sealed record ChainData(
            [property: JsonPropertyName("calls")] List<ContractDetails> Calls,
            [property: JsonPropertyName("puts")] List<ContractDetails> Puts 
        );

        private sealed record ContractDetails(
            [property: JsonPropertyName("contract")] string Contract,
            [property: JsonPropertyName("strike")] double Strike,
            [property: JsonPropertyName("lastPrice")] double? LastPrice,
            [property: JsonPropertyName("bid")] double? Bid,
            [property: JsonPropertyName("ask")] double? Ask,
            [property: JsonPropertyName("volume")] int? Volume,
            [property: JsonPropertyName("openInterest")] int? OpenInterest,
            [property: JsonPropertyName("impliedVol")] double? ImpliedVol
        );

        private static bool IsValidData(ContractDetails contractDetails) {
            if (contractDetails.Bid == 0 || contractDetails.Ask == 0) {
                return false;
            }
            return true;
        }

        public async Task<Dictionary<Underlying, ILocalVolatilityModel>> GetVolatilitiesAsync(IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", ".."));
            string path = Path.Combine(projectRoot, _path);
            var json = await File.ReadAllTextAsync(path, ct);
            Dictionary<string, StockOptions> jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, StockOptions>>(json, JsonOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize spots from: {path}");

            Dictionary<Underlying, Dictionary<VanillaContract, double>> pricesByUnderlying = new();
            foreach (Underlying underlying in underlyings) {
                Dictionary<VanillaContract, double> optionPrices = new();
                if (jsonDictionary.ContainsKey(underlying.Code)) {
                    StockOptions entry = jsonDictionary[underlying.Code];
                    foreach (String date in entry.Chains.Keys) {
                        ChainData chain = entry.Chains[date];
                        DateTime maturity = DateTime.Parse(date);
                        foreach (ContractDetails call in chain.Calls) {
                            if (IsValidData(call)) {
                                EuropeanCall c = new() {
                                    Underlying = underlying,
                                    Maturity = maturity,
                                    Strike = call.Strike,
                                    Currency = underlying.Currency
                                };
                                optionPrices[c] = call.LastPrice ?? 0;
                            }
                        }
                        foreach (ContractDetails put in chain.Puts) {
                            if (IsValidData(put)) {
                                EuropeanPut p = new() {
                                    Underlying = underlying,
                                    Maturity = maturity,
                                    Strike = put.Strike,
                                    Currency = underlying.Currency
                                };
                                optionPrices[p] = put.LastPrice ?? 0;
                            }
                        }
                    }
                }
                pricesByUnderlying[underlying] = optionPrices;
            }
            Dictionary<Underlying, ILocalVolatilityModel> volatilitiesByUnderlying = new();
            foreach (Underlying underlying in underlyings) {
                if (pricesByUnderlying.ContainsKey(underlying)) {
                    Dictionary<VanillaContract, double> optionPrices = pricesByUnderlying[underlying];
                    ILocalVolatilityModel model = _builder.BuildVolatilitySurface(optionPrices);
                    volatilitiesByUnderlying[underlying] = model;
                }
            }
            return volatilitiesByUnderlying;
        }
    }
}
