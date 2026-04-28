using Domain;

namespace Application {
    public class ShiftedVolatilityModel : ILocalVolatilityModel {
        private readonly ILocalVolatilityModel _volatilityModel;
        private readonly double _volatilityShift;

        public ShiftedVolatilityModel(ILocalVolatilityModel volatilityModel, double volatilityShift) {
            _volatilityModel = volatilityModel;
            _volatilityShift = volatilityShift;
        }

        public double getVolatility(double spot, double timeToMaturity) {
            return _volatilityModel.getVolatility(spot, timeToMaturity) + _volatilityShift;
        }
    }
}
