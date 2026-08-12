using Domain;

namespace Application {
    public interface IVolatilitySurfaceBuilder {
        IImpliedVolatilityModel BuildVolatilitySurface(Dictionary<VanillaContract, double> optionPrices);
    }
}
