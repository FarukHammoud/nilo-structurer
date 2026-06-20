namespace Domain {
    /// <summary>
    /// Cox-Ingersoll-Ross (CIR) model for short rate dynamics.
    /// </summary>
    public class CoxIngersollRoss {
        private readonly double _kappa;  // mean reversion speed
        private readonly double _theta;  // long-term mean
        private readonly double _sigma;  // volatility
        private readonly double _gamma;  // sqrt(kappa^2 + 2*sigma^2)

        public CoxIngersollRoss(double kappa, double theta, double sigma) {
            _kappa = kappa;
            _theta = theta;
            _sigma = sigma;
            _gamma = Math.Sqrt(kappa * kappa + 2 * sigma * sigma);
        }

        private double Denom(double tau)
            => (_gamma + _kappa) * (Math.Exp(_gamma * tau) - 1) + 2 * _gamma;

        public double B(double tau)
            => 2 * (Math.Exp(_gamma * tau) - 1) / Denom(tau);

        public double A(double tau) {
            double numerator = 2 * _gamma * Math.Exp((_kappa + _gamma) * tau / 2);
            return Math.Pow(numerator / Denom(tau), 2 * _kappa * _theta / (_sigma * _sigma));
        }

        public double DiscountFactor(double r, double tau)
            => A(tau) * Math.Exp(-B(tau) * r);
    }
}
