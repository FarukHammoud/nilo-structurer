using Application;
using Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure {
    public class JsonOptionPriceProvider : IOptionPriceProvider {
        private readonly string _path;

        private static readonly JsonSerializerOptions JsonOptions = new() {
            PropertyNameCaseInsensitive = true,
        };

        public JsonOptionPriceProvider(string path) {
            _path = path;
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
        public async Task<Dictionary<Underlying, Dictionary<VanillaContract, OptionMarketData>>> GetOptionPricesAsync(IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", ".."));
            string path = Path.Combine(projectRoot, _path);
            var json = await File.ReadAllTextAsync(path, ct);
            Dictionary<string, StockOptions> jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, StockOptions>>(json, JsonOptions)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize spots from: {path}");

            Dictionary<Underlying, Dictionary<VanillaContract, OptionMarketData>> pricesByUnderlying = new();
            foreach (Underlying underlying in underlyings) {
                Dictionary<VanillaContract, OptionMarketData> optionPrices = new();
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
                                OptionMarketData marketData = new() {
                                    LastPrice = call.LastPrice ?? 0,
                                    Bid = call.Bid ?? 0,
                                    Ask = call.Ask ?? 0,
                                    Volume = call.Volume ?? 0,
                                    OpenInterest = call.OpenInterest ?? 0
                                };
                                optionPrices[c] = marketData;
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
                                OptionMarketData marketData = new() {
                                    LastPrice = put.LastPrice ?? 0,
                                    Bid = put.Bid ?? 0,
                                    Ask = put.Ask ?? 0,
                                    Volume = put.Volume ?? 0,
                                    OpenInterest = put.OpenInterest ?? 0
                                };
                                optionPrices[p] = marketData;
                            }
                        }
                    }
                }
                pricesByUnderlying[underlying] = optionPrices;
            }
            return pricesByUnderlying;
        }
    }
}
