using Domain;

namespace Application {
    /// <summary>
    /// sigma_local = sigma * (1 + beta / S)
    /// Equivalent to dS = mu * S * dt + sigma * (S + beta) * dW
    /// </summary>
    public class InverseLinearVolatilityModel : ILocalVolatilityModel {
        private double _volatility;
        private double _beta;
        private double _riskfreeRate;
        public InverseLinearVolatilityModel(double volatility, double beta, double riskFreeRate) {
            _volatility = volatility;
            _beta = beta;
            _riskfreeRate = riskFreeRate;
        }
        public double GetVolatility(double spot, double timeToMaturity) {
            double beta_t = _beta * Math.Exp(-_riskfreeRate * timeToMaturity);
            return _volatility * (1.0 + beta_t / spot);
        }
    }
}
