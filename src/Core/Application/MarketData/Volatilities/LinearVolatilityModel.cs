using Domain;

namespace Application {
    public class LinearVolatilityModel : ILocalVolatilityModel {
        private double _volatility;
        private double _skew;
        private double _termStructure;
        private double _spotReference;
        public LinearVolatilityModel(double volatility, double skew, double termStructure, double spotReference) {
            this._volatility = volatility;
            this._skew = skew;
            this._termStructure = termStructure;
            this._spotReference = spotReference;
        }
        public double GetVolatility(double spot, double timeToMaturity) {
            return _volatility + _skew * (spot - _spotReference) + _termStructure * timeToMaturity;
        }
    }
}
