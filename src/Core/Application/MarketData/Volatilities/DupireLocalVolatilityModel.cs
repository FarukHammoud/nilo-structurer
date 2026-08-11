using Domain;

namespace Application {
    public class DupireLocalVolatilityModel : ILocalVolatilityModel {
        private IImpliedVolatilityModel _impliedVolatilityModel;
        private Dupire _dupireModel;
        public DupireLocalVolatilityModel(IImpliedVolatilityModel impliedVolatilityModel, IDiscounter discounter) {
            _dupireModel = new Dupire(_impliedVolatilityModel, discounter);
        }
        public double GetVolatility(double spot, double timeToMaturity) {
            return _dupireModel.GetLocalVolatility(spot, DateTime.Today.AddDays(timeToMaturity * 365), DateTime.Today);
        }
    }
}
