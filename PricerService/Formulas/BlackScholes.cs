
using MathNet.Numerics.Distributions;

namespace PricerServices {
    public static class BlackScholes {

        public static Func<double, double> N = x => Normal.CDF(0, 1, x);
        public static Func<double, double> Exp = x => Math.Exp(x);
        public static double d1(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double sigma = volatility;
            return (Math.Log(S / K) + (r + 0.5 *sigma * sigma) * T) / (sigma * Math.Sqrt(T));
        }

        public static double d2(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double sigma = volatility;
            return d1(S, K, T, r, sigma) - sigma * Math.Sqrt(T);
        }

        public static double DigitalCallPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double sigma = volatility;
            double D2 = d2(S, K, T, r, sigma);
            return Exp(-r * T) * N(D2);
        }

        public static double DigitalPutPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double sigma = volatility;
            double D2 = d2(S, K, T, r, sigma);
            return Exp(-r * T) * (1 - N(D2));
        }

        public static double CallPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double sigma = volatility;
            double D1 = d1(S, K, T, r, sigma);
            double D2 = d2(S, K, T, r, sigma);
            return S * N(D1) - K * Exp(-r * T) * N(D2);
        }

        public static double PutPrice(double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            double S = spot;
            double K = strike;
            double T = timeToMaturity;
            double r = riskFreeRate;
            double sigma = volatility;
            double D1 = d1(S, K, T, r, sigma);
            double D2 = d2(S, K, T, r, sigma);
            return K * Exp(-r * T) * N(-D2) - S * N(-D1);
        }
    }
}
