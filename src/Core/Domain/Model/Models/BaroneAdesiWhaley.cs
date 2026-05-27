using MathNet.Numerics.Distributions;

namespace Domain {
    // Barone-Adesi Whaley (BAW) Quadratic Approximation for American Options
    // Reference: "Analytic Approximations for American Options", Journal of Finance, Vol. 42, No. 2 (1987), pp. 301-320
    public static class BaroneAdesiWhaley {

        public static double PriceAmerican(OptionType optionType, double S, double K, double r, double sigma, double T) {
            // For a non-dividend paying stock, an American Call is equal to a European Call
            if (optionType == OptionType.Call) {
                return new BlackScholes(OptionType.Call, S, K, T, r, sigma).Premium;
            }

            // --- American Put Approximation ---
            double euroPrice = new BlackScholes(OptionType.Put, S, K, T, r, sigma).Premium;
            double sigmaSq = sigma * sigma;
            double N = 2.0 * r / sigmaSq;
            double K_factor = 1.0 - Math.Exp(-r * T);

            double q2 = (-(N - 1) - Math.Sqrt(Math.Pow(N - 1, 2) + (4.0 * N / K_factor))) / 2.0;

            // Newton seed calculation safe for low-interest rates
            double S_star = K * Math.Min(1.0, 2.0 * r / (2.0 * r + sigmaSq));
            if (S_star >= K || S_star <= 0) S_star = K * 0.9;

            double error = 1e-8;
            double diff = 1.0;

            for (int i = 0; i < 100 && Math.Abs(diff) > error; i++) {
                double d1 = (Math.Log(S_star / K) + (r + 0.5 * sigmaSq) * T) / (sigma * Math.Sqrt(T));
                double euroPut_Sstar = new BlackScholes(OptionType.Put, S_star, K, T, r, sigma).Premium;

                double lhs = K - S_star - euroPut_Sstar;
                double rhs = -(S_star / q2) * (1.0 - Math.Exp(-r * T) * Normal.CDF(0, 1, -d1));
                double fn = lhs - rhs;

                double fn_prime = -1.0 + Normal.CDF(0, 1, -d1)
                                  + (1.0 / q2) * (1.0 - Math.Exp(-r * T) * Normal.CDF(0, 1, -d1))
                                  - (Math.Exp(-r * T) * Normal.PDF(0, 1, -d1)) / (q2 * sigma * Math.Sqrt(T));

                if (Math.Abs(fn_prime) < 1e-10) break;

                diff = fn / fn_prime;
                double next_S_star = S_star - diff;

                // Keep S_star bounded inside realistic financial boundaries
                if (next_S_star <= 0 || next_S_star > K) break;
                S_star = next_S_star;
            }

            if (S <= S_star) {
                return K - S;
            } else {
                double d1_Sstar = (Math.Log(S_star / K) + (r + 0.5 * sigmaSq) * T) / (sigma * Math.Sqrt(T));
                double A2 = -(S_star / q2) * (1.0 - Math.Exp(-r * T) * Normal.CDF(0, 1, -d1_Sstar));
                return euroPrice + A2 * Math.Pow(S / S_star, q2);
            }
        }
    }
}
