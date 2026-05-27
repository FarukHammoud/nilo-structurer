
using MathNet.Numerics.Distributions;

namespace Domain {
    public class BlackScholes {

        private static Func<double, double> N = x => Normal.CDF(0, 1, x);
        private static Func<double, double> n = x => Normal.PDF(0, 1, x);
        private static Func<double, double> Exp = Math.Exp;

        private readonly OptionType _optionType;
        private readonly double S;
        private readonly double K;
        private readonly double T;
        private readonly double r;
        private readonly double σ;
        private readonly double b;
        public double d1 => (Math.Log(S / K) + (b + 0.5 * σ * σ) * T) / (σ * Math.Sqrt(T));
        public double d2 => d1 - σ * Math.Sqrt(T);

        public double Theta {
            get {
                double term1 = -(S * σ * Exp((b - r) * T) * n(d1)) / (2 * Math.Sqrt(T));
                if (_optionType == OptionType.Call)
                    return term1 + (r - b) * S * Exp((b - r) * T) * N(+d1) - r * K * Exp(-r * T) * N(+d2);
                else
                    return term1 + (b - r) * S * Exp((b - r) * T) * N(-d1) + r * K * Exp(-r * T) * N(-d2);
            }
        }
        public double Rho => _optionType == OptionType.Call 
            ? +K * T * Exp(-r * T) * N(+d2) 
            : -K * T * Exp(-r * T) * N(-d2);
        public double Gamma => Exp((b - r) * T) * n(d1) / (S * σ * Math.Sqrt(T));
        public double Delta => _optionType == OptionType.Call 
            ? Exp((b - r) * T) * N(d1) 
            : Exp((b - r) * T) * (N(d1) - 1);
        public double Premium => _optionType == OptionType.Call 
            ? S * Exp((b - r) * T) * N(+d1) - K * Exp(-r * T) * N(+d2) 
            : K * Exp(-r * T) * N(-d2) - S * Exp((b - r) * T) * N(-d1);
        public double Vega => S * Exp((b - r) * T) * Math.Sqrt(T) * n(d1);
        public double Forward => S * Exp(b * T);

        public BlackScholes(OptionType optionType, double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility, double? costOfCarry = null) {
            _optionType = optionType;
            S = spot;
            K = strike;
            T = timeToMaturity;
            r = riskFreeRate;
            σ = volatility;
            b = costOfCarry.HasValue ? costOfCarry.Value : riskFreeRate; // risk-neutral drift in general
        }

        public double GetImpliedVolatility(double marketPrice, double tolerance = 1e-6, int maxIterations = 1000) {
            return GetExplicitImpliedVolatility();
        }

        public double GetImpliedVolatilityIteratively(double marketPrice, double tolerance = 1e-6, int maxIterations = 1000) {
            double σLower = 1e-6;
            double σUpper = 5.0;
            double σMid = (σLower + σUpper) / 2;
            for (int i = 0; i < maxIterations; i++) {
                double priceMid = new BlackScholes(_optionType, S, K, T, r, σMid, b).Premium;
                if (Math.Abs(priceMid - marketPrice) < tolerance) {
                    return σMid;
                }
                if (priceMid < marketPrice) {
                    σLower = σMid;
                } else {
                    σUpper = σMid;
                }
                σMid = (σLower + σUpper) / 2;
            }
            return σMid;
        }

        public double GetExplicitImpliedVolatility() {
            // An Explicit Solution to Black–Scholes Implied Volatility
            // Wolfgang Schadner (2026)
            double discountFactor = Exp(-r * T);
            double forwardPrice = S * Exp(b * T);
            double normalizedPrice = Premium / (discountFactor * forwardPrice);
            double forwardLogMoneyness = Math.Log(K / forwardPrice);

            // Case 1: At-the-forward (k == 0)
            if (Math.Abs(forwardLogMoneyness) < 1e-6) {
                // Collapses to standard normal inverse CDF
                return (2.0 / Math.Sqrt(T)) * Normal.InvCDF(0.0, 1.0, (normalizedPrice + 1.0) / 2.0);
            }

            // Case 2 & 3: Out-of-the-money (k > 0) and In-the-money (k < 0)
            double m = (K > forwardPrice) ? 1.0 : K / forwardPrice;
            double mu = 2.0 / Math.Abs(forwardLogMoneyness);
            double lambda = 1.0; // The standard shape parameter used in the paper

            double qLevel = (1.0 - normalizedPrice) / m;
            if (_optionType == OptionType.Put) {
                qLevel = (Exp(forwardLogMoneyness) - normalizedPrice) / m;
            }
            
            double inverseGaussianQuantile = InverseGaussian.InvCDF(mu, lambda, qLevel);

            // Final Explicit Volatility calculation
            return (2.0 / Math.Sqrt(T)) * Math.Pow(inverseGaussianQuantile, -0.5);
        }

        public double DigitalCallPrice() {
            return Exp(-r * T) * N(d2);
        }

        public double DigitalPutPrice() {
            return Exp(-r * T) * N(-d2);
        }
    }
}
