
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
        public double d1 => (Math.Log(S / K) + (r + 0.5 * σ * σ) * T) / (σ * Math.Sqrt(T));
        public double d2 => d1 - σ * Math.Sqrt(T);

        public double Theta => -S * n(d1) * σ / (2 * Math.Sqrt(T)) - r * K * Exp(-r * T) * N(d2);
        public double Rho => _optionType == OptionType.Call ? K * T * Exp(-r * T) * N(d2) : - K * T * Exp(-r * T) * N(-d2);
        public double Gamma => n(d1) / (S * σ * Math.Sqrt(T));
        public double Delta => _optionType == OptionType.Call ? N(d1) : 1 - N(d1);
        public double Premium => _optionType == OptionType.Call ? S * N(d1) - K * Exp(-r * T) * N(d2) : K * Exp(-r* T) * N(-d2) - S * N(-d1);

        public BlackScholes(OptionType optionType, double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            _optionType = optionType;
            S = spot;
            K = strike;
            T = timeToMaturity;
            r = riskFreeRate;
            σ = volatility;
        }

        public double DigitalCallPrice() {
            return Exp(-r * T) * N(d2);
        }

        public double DigitalPutPrice() {
            return Exp(-r * T) * (1 - N(d2));
        }
    }
}
