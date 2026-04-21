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
            double C1 = BlackScholes.CallPrice(S1, K, T, r, σ1);
            double C2 = BlackScholes.CallPrice(S2, K, T, r, σ2);
            return C1 + C2 - CallWorstOf(S1, S2, K, r, σ1, σ2, ρ, T);
        }

        public static double CallWorstOf(double S1, double S2, double K,
                               double r, double σ1, double σ2,
                               double ρ, double T) {
            double D1_1 = BlackScholes.d1(S1, K, T, r, σ1);
            double D2_1 = BlackScholes.d2(S1, K, T, r, σ1);
            double D1_2 = BlackScholes.d1(S2, K, T, r, σ2);
            double D2_2 = BlackScholes.d2(S2, K, T, r, σ2);
            double sigmaRatio = Math.Sqrt(σ1 * σ1 + σ2 * σ2 - 2 * ρ * σ1 * σ2);
            double e1 = BlackScholes.d1(S1, S2, T, 0, sigmaRatio);
            double e2 = -BlackScholes.d2(S1, S2, T, 0, sigmaRatio);
            double ρS1Ratio = (σ1 - ρ * σ2) / sigmaRatio;
            double ρS2Ratio = (σ2 - ρ * σ1) / sigmaRatio;
            return S1 * N2(D1_1, -e1, -ρS1Ratio) + S2 * N2(D1_2, -e2, -ρS2Ratio)
                 - K * Exp(-r * T) * N2(D2_1, D2_2, ρ);
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
