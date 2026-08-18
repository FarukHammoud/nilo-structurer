using Domain;

namespace Application {
    public class StochasticVolatilityModel : IStochasticVolatility {
        private Realizations _varianceRealizations;
        public StochasticVolatilityModel(Realizations varianceRealizations) {
            _varianceRealizations = varianceRealizations;
        }
        public double GetVolatility(int ω, int step) {
            return Math.Sqrt(Math.Max(0.0, _varianceRealizations[ω].Values[step]));
        }
    }
}
