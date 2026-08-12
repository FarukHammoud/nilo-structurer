using Domain;

namespace Application {
    public class DupireLocalVolatilityModel : ILocalVolatilityModel {
        private IImpliedVolatilityModel _impliedVolatilityModel;
        private Dupire _dupireModel;
        private double _referenceSpot;
        public DupireLocalVolatilityModel(IImpliedVolatilityModel impliedVolatilityModel, IDiscounter discounter, double referenceSpot) {
            _impliedVolatilityModel = impliedVolatilityModel;
            _dupireModel = new Dupire(impliedVolatilityModel, discounter);
            _referenceSpot = referenceSpot;
        }
        public double GetVolatility(double spot, DateTime time) {
            // Fall back to the implied volatility model if it is a constant volatility model
            if (_impliedVolatilityModel is ConstantVolatilityModel || _impliedVolatilityModel is MertonJumpModel) {
                return _impliedVolatilityModel.GetVolatility(spot, time);
            }
            return _dupireModel.GetLocalVolatility(spot, time, _referenceSpot, DateTime.Today);
        }
    }
}
