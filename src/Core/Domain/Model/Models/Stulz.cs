using MathNet.Numerics.Distributions;
using MathNet.Numerics.Integration;

namespace Domain {
    /// <summary>
    /// "Options on the Minimum or the Maximum of Two Risky Assets", Journal of Financial Economics 10 (1982), 161–185
    /// https://u.osu.edu/stulz.1/files/2016/05/Options-on-the-Minimum-or-Maximum-q6ixkg.pdf
    /// </summary>
    public static class Stulz {

        public static Func<double, double> N = x => Normal.CDF(0, 1, x);
        public static Func<double, double> n = x => Normal.PDF(0, 1, x);
        public static Func<double, double> Exp = Math.Exp;

        // This method computes the bivariate normal cumulative distribution function using Gauss-Legendre quadrature.
        public static double N2(double a, double b, double ρ) {
            double f(double b, double ρ, double z) {
                return N((b - ρ * z) / Math.Sqrt(1 - ρ * ρ)) * n(z);
            }
            return GaussLegendreRule.Integrate(z => f(b, ρ, z), -10, a, 10);
        }

        public static double CallBestOf(double S1, double S2, double K,
                               double r, double σ1, double σ2,
                               double ρ, double T) {
            double C1 = new BlackScholes(OptionType.Call, S1, K, T, r, σ1).Premium;
            double C2 = new BlackScholes(OptionType.Call, S2, K, T, r, σ2).Premium;
            return C1 + C2 - CallWorstOf(S1, S2, K, r, σ1, σ2, ρ, T);
        }

        public static double PutBestOf(double S1, double S2, double K,
                       double r, double σ1, double σ2,
                       double ρ, double T) {
            double P1 = new BlackScholes(OptionType.Put, S1, K, T, r, σ1).Premium;
            double P2 = new BlackScholes(OptionType.Put, S2, K, T, r, σ2).Premium;
            return P1 + P2 - PutWorstOf(S1, S2, K, r, σ1, σ2, ρ, T);
        }

        // Option to exchange one asset for another (i.e. strike = 0)
        // Margrabe (1978)
        private static double CallWorstOfZeroStrike(double S1, double S2,
                       double r, double σ1, double σ2,
                       double ρ, double T) {
            double σRatio = Math.Sqrt(σ1 * σ1 + σ2 * σ2 - 2 * ρ * σ1 * σ2);
            double d1 = (Math.Log(S1 / S2) + 0.5 * σRatio * σRatio * T) / (σRatio * Math.Sqrt(T));
            double d2 = d1 - σRatio * Math.Sqrt(T);
            return S1 - S1 * N(d1) + S2 * N(d2);
        }

        public static double CallWorstOf(double S1, double S2, double K,
                               double r, double σ1, double σ2,
                               double ρ, double T) {
            double D1_1 = new BlackScholes(OptionType.Call, S1, K, T, r, σ1).d1;
            double D2_1 = new BlackScholes(OptionType.Call, S1, K, T, r, σ1).d2;
            double D1_2 = new BlackScholes(OptionType.Call, S2, K, T, r, σ2).d1;
            double D2_2 = new BlackScholes(OptionType.Call, S2, K, T, r, σ2).d2;
            double σRatio = Math.Sqrt(σ1 * σ1 + σ2 * σ2 - 2 * ρ * σ1 * σ2);
            double e1 = new BlackScholes(OptionType.Call, S1, S2, T, 0, σRatio).d1;
            double e2 = -new BlackScholes(OptionType.Call, S1, S2, T, 0, σRatio).d2;
            double ρS1Ratio = (σ1 - ρ * σ2) / σRatio;
            double ρS2Ratio = (σ2 - ρ * σ1) / σRatio;
            return S1 * N2(D1_1, -e1, -ρS1Ratio) + S2 * N2(D1_2, -e2, -ρS2Ratio)
                 - K * Exp(-r * T) * N2(D2_1, D2_2, ρ);
        }

        public static double PutWorstOf(double S1, double S2, double K,
                       double r, double σ1, double σ2,
                       double ρ, double T) {
            return K * Exp(-r * T)
                - CallWorstOfZeroStrike(S1, S2, r, σ1, σ2, ρ, T)  // M(V,H,0,τ)
                + CallWorstOf(S1, S2, K, r, σ1, σ2, ρ, T); // M(V,H,F,τ)
        }

        public static double DoubleDigital2D(double X, double Y, double Kx, double Ky,
                               double r, double σx, double σy,
                               double ρ, double T) {
            double d2X = (Math.Log(X / Kx) + (r - 0.5 * σx * σx) * T)
                         / (σx * Math.Sqrt(T));
            double d2Y = (Math.Log(Y / Ky) + (r - 0.5 * σy * σy) * T)
                         / (σy * Math.Sqrt(T));

            return Exp(-r * T) * N2(d2X, d2Y, ρ);
        }
    }
}
