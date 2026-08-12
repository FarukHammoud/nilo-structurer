using Domain;

namespace Application {
    public class ShiftedVolatilityModel : IImpliedVolatilityModel {
        private readonly IImpliedVolatilityModel _volatilityModel;
        private readonly double _volatilityShift;

        public ShiftedVolatilityModel(IImpliedVolatilityModel volatilityModel, double volatilityShift) {
            _volatilityModel = volatilityModel;
            _volatilityShift = volatilityShift;
        }

        public double GetVolatility(double spot, DateTime time) {
            return _volatilityModel.GetVolatility(spot, time) + _volatilityShift;
        }
    }
}
