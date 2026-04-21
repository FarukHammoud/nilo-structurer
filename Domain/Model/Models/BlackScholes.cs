
using MathNet.Numerics.Distributions;

namespace Domain {
    public static class BlackScholes {

        public static Func<double, double> N = x => Normal.CDF(0, 1, x);
        public static Func<double, double> n = x => Normal.PDF(0, 1, x);
        public static Func<double, double> Exp = Math.Exp;
        public static double d1(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            return (Math.Log(S / K) + (r + 0.5 * σ * σ) * T) / (σ * Math.Sqrt(T));
        }

        public static double d2(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            return d1(S, K, T, r, σ) - σ * Math.Sqrt(T);
        }

        public static double DigitalCallPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            double D2 = d2(S, K, T, r, σ);
            return Exp(-r * T) * N(D2);
        }

        public static double DigitalPutPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            double D2 = d2(S, K, T, r, σ);
            return Exp(-r * T) * (1 - N(D2));
        }

        public static double CallPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            double D1 = d1(S, K, T, r, σ);
            double D2 = d2(S, K, T, r, σ);
            return S * N(D1) - K * Exp(-r * T) * N(D2);
        }

        public static double CallDelta(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            double D1 = d1(S, K, T, r, σ);
            return N(D1);
        }

        public static double PutDelta(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            double D1 = d1(S, K, T, r, σ);
            return 1 - N(D1);
        }

        public static double PutPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double σ = volatility;
            double D1 = d1(S, K, T, r, σ);
            double D2 = d2(S, K, T, r, σ);
            return K * Exp(-r * T) * N(-D2) - S * N(-D1);
        }
    }
}
