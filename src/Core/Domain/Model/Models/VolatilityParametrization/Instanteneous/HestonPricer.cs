using System;
using System.Numerics;
using MathNet.Numerics.Integration;

namespace Domain {
    
    public static class HestonPricer {
        /// <summary>
        /// Prices a European Call Option under the Heston Model 
        /// </summary>
        /// <param name="S0">Current Spot Price</param>
        /// <param name="K">Strike Price</param>
        /// <param name="T">Time to Maturity (years)</param>
        /// <param name="r">Risk-free Rate</param>
        /// <param name="v0">Current Spot Variance</param>
        /// <param name="rho">Correlation between asset and volatility</param>
        /// <param name="heston">Domain Heston parameter container (λ, v_, η)</param>
        public static double PriceCall(
            double S0, double K, double T, double r, double v0, double rho, Heston heston) {
            // P1 (delta) and P2 (exercise probability) integrations
            double p1 = 0.5 + (1.0 / Math.PI) * GaussLegendreRule.Integrate(
            u => Integrand(u, 1, S0, K, T, r, v0, rho, heston), 0.00001, 100.0, 32);

            double p2 = 0.5 + (1.0 / Math.PI) * GaussLegendreRule.Integrate(
            u => Integrand(u, 2, S0, K, T, r, v0, rho, heston), 0.00001, 100.0, 32);

            // Black-Scholes style payoff equation
            double price = S0 * p1 - K * Math.Exp(-r * T) * p2;

            // Ensure result does not violate lower intrinsic bound
            return Math.Max(price, Math.Max(0.0, S0 - K * Math.Exp(-r * T)));
        }

        private static double Integrand(
            double u, int type, double S0, double K, double T, double r, double v0, double rho, Heston heston) {
            Complex i = Complex.ImaginaryOne;

            Complex phi = (type == 1)
            ? CharacteristicFunction(u - i, S0, T, r, v0, rho, heston) / CharacteristicFunction(-i, S0, T, r, v0, rho, heston)
            : CharacteristicFunction(u, S0, T, r, v0, rho, heston);

            Complex element = Complex.Exp(-i * u * Math.Log(K)) * phi / (i * u);
            return element.Real;
        }

        // Little Trap Heston Characteristic Function (Albrecher et al., 2007)
        private static Complex CharacteristicFunction(
            Complex u, double S0, double T, double r, double v0, double rho, Heston heston) {
            Complex i = Complex.ImaginaryOne;

            // Mapping your Heston class properties:
            // heston.λ  = Speed of reversion (kappa)
            // heston.v_ = Long-term mean variance (theta)
            // heston.η  = Volatility of volatility (sigma)

            Complex xi = heston.λ - rho * heston.η * i * u;
            Complex d = Complex.Sqrt(xi * xi + heston.η * heston.η * (u * u + i * u));
            Complex g = (xi - d) / (xi + d);

            Complex C = r * i * u * T + (heston.λ * heston.v_ / (heston.η * heston.η)) *
                    ((xi - d) * T - 2.0 * Complex.Log((1.0 - g * Complex.Exp(-d * T)) / (1.0 - g)));

            Complex D = ((xi - d) / (heston.η * heston.η)) * ((1.0 - Complex.Exp(-d * T)) / (1.0 - g * Complex.Exp(-d * T)));

            return Complex.Exp(C + D * v0 + i * u * Math.Log(S0));
        }
    }
}
