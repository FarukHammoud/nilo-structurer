using Domain;

namespace Application {
    public interface IVolatilitySurfaceBuilder {
        ILocalVolatilityModel BuildVolatilitySurface(Dictionary<VanillaContract, double> optionPrices);
    }
}
