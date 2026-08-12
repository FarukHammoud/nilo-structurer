using Domain;

namespace Application {
    public class LinearVolatilityModel : IImpliedVolatilityModel, ILocalVolatilityModel {
        private double _volatility;
        private double _skew;
        private double _termStructure;
        private double _spotReference;
        private IDayCountConvention _dayCounter = new Actual365();
        public LinearVolatilityModel(double volatility, double skew, double termStructure, double spotReference) {
            this._volatility = volatility;
            this._skew = skew;
            this._termStructure = termStructure;
            this._spotReference = spotReference;
        }
        public double GetVolatility(double spot, DateTime time) {
            // Assuming timeToMaturity can be derived from the DateTime
            double timeToMaturity = _dayCounter.YearFraction(DateTime.Today, time);
            return _volatility + _skew * (spot - _spotReference) + _termStructure * timeToMaturity;
        }
    }
}
