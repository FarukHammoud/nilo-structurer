using Domain;

namespace Application {
    public class ShiftedVolatilityModel : ILocalVolatilityModel {
        private readonly ILocalVolatilityModel _volatilityModel;
        private readonly double _volatilityShift;

        public ShiftedVolatilityModel(ILocalVolatilityModel volatilityModel, double volatilityShift) {
            _volatilityModel = volatilityModel;
            _volatilityShift = volatilityShift;
        }

        public double GetVolatility(double spot, double timeToMaturity) {
            return _volatilityModel.GetVolatility(spot, timeToMaturity) + _volatilityShift;
        }
    }
}
