using Domain;

namespace Application {
    /// <summary>
    /// dS = μ * S * dt + σ * (S + β(t)) * dW [Displaced Diffusion]
    /// dS/S = μ * dt + [σ * (1 + β(t)/S)] * dW
    /// σ_local(S,t) = [σ * (1 + β(t)/S)]
    /// </summary>
    public class InverseLinearVolatilityModel : ILocalVolatilityModel, IImpliedVolatilityModel {
        private double _volatility;
        private double _beta;
        private double _riskfreeRate;
        private DateTime _today;
        private IDayCountConvention _dayCountConvention = new Actual365();

        public InverseLinearVolatilityModel(double volatility, double beta, double riskFreeRate, DateTime today) {
            _volatility = volatility;
            _beta = beta;
            _riskfreeRate = riskFreeRate;
            _today = today;
        }

        public double GetVolatility(double spot, DateTime date) {
            double t = _dayCountConvention.YearFraction(_today, date);
            double beta_t = _beta * Math.Exp(-_riskfreeRate * t);
            return _volatility * (1.0 + beta_t / spot);
        }
    }
}
