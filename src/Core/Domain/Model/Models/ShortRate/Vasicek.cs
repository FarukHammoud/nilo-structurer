namespace Domain {
    public class Vasicek {
        private readonly double _kappa;  // mean reversion speed
        private readonly double _theta;  // long-term mean
        private readonly double _sigma;  // volatility

        public Vasicek(double kappa, double theta, double sigma) {
            _kappa = kappa;
            _theta = theta;
            _sigma = sigma;
        }

        public double B(double tau)
            => (1 - Math.Exp(-_kappa * tau)) / _kappa;

        public double A(double tau) {
            double b = B(tau);
            double sigma2 = _sigma * _sigma;
            return Math.Exp(
                (_theta - sigma2 / (2 * _kappa * _kappa)) * (b - tau)
                - sigma2 / (4 * _kappa) * b * b
            );
        }

        public double DiscountFactor(double r, double tau)
            => A(tau) * Math.Exp(-B(tau) * r);
    }
}
