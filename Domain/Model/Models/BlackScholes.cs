
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

        public BlackScholes(OptionType optionType, double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility, double? costOfCarry = null) {
            _optionType = optionType;
            S = spot;
            K = strike;
            T = timeToMaturity;
            r = riskFreeRate;
            σ = volatility;
            b = costOfCarry.HasValue ? costOfCarry.Value : riskFreeRate;
        }

        public double DigitalCallPrice() {
            return Exp(-r * T) * N(d2);
        }

        public double DigitalPutPrice() {
            return Exp(-r * T) * N(-d2);
        }
    }
}
