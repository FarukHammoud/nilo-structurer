using Domain;

namespace Application {
    public class InterpolationVolatilitySurfaceBuilder : IVolatilitySurfaceBuilder {
        public ILocalVolatilityModel BuildVolatilitySurface(Dictionary<VanillaContract, double> optionPrices) {
            Dictionary<EuropeanCall, double> callPrices = optionPrices.Where(kvp => kvp.Key is EuropeanCall)
                .ToDictionary(kvp => (EuropeanCall)kvp.Key, kvp => kvp.Value);
            Dictionary<EuropeanPut, double> putPrices = optionPrices.Where(kvp => kvp.Key is EuropeanPut)
                .ToDictionary(kvp => (EuropeanPut)kvp.Key, kvp => kvp.Value);

            // Implementation for building the volatility surface using interpolation
            throw new NotImplementedException();
        }
    }
}
