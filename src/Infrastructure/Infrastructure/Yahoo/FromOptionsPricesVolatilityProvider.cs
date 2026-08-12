using Application;
using Domain;

namespace Infrastructure {
    public class FromOptionsPricesVolatilityProvider : IVolatilityProvider {
        private readonly IVolatilitySurfaceBuilder _builder;
        private readonly IOptionPriceProvider _optionPriceProvider;

        public FromOptionsPricesVolatilityProvider(IOptionPriceProvider optionPriceProvider, IVolatilitySurfaceBuilder builder) {
            _builder = builder;
            _optionPriceProvider = optionPriceProvider;
        }

        public async Task<Dictionary<Underlying, IImpliedVolatilityModel>> GetVolatilitiesAsync(IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            Dictionary<Underlying, Dictionary<VanillaContract, OptionMarketData>> pricesByUnderlying = await _optionPriceProvider.GetOptionPricesAsync(underlyings, ct);
            Dictionary<Underlying, IImpliedVolatilityModel> volatilitiesByUnderlying = new();
            foreach (Underlying underlying in underlyings) {
                if (pricesByUnderlying.ContainsKey(underlying)) {
                    Dictionary<VanillaContract, OptionMarketData> optionData = pricesByUnderlying[underlying];
                    Dictionary<VanillaContract, double> optionPrices = optionData.ToDictionary(x => x.Key, x => x.Value.LastPrice);
                    IImpliedVolatilityModel model = _builder.BuildVolatilitySurface(optionPrices);
                    volatilitiesByUnderlying[underlying] = model;
                }
            }
            return volatilitiesByUnderlying;
        }
    }
}
