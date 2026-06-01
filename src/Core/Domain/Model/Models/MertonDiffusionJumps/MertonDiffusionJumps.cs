
using MathNet.Numerics;

namespace Domain {
    public class MertonDiffusionJumps {
        private static Func<double, double> Exp = Math.Exp;
        private static Func<double, double, double> Pow = Math.Pow;
        private static Func<double, double> Sqrt = Math.Sqrt;
        private static Func<int, double> Fact = SpecialFunctions.Factorial;
        private readonly JumpParameters _jumpParameters;
        private readonly OptionType _optionType;
        private readonly double S;
        private readonly double K;
        private readonly double T;
        private readonly double r;
        private readonly double σ;
        private readonly double b;
        private readonly double λ;
        private readonly double μJ;
        private readonly double σJ; 

        public MertonDiffusionJumps(JumpParameters jumpParameters, OptionType optionType, double spot, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            _jumpParameters = jumpParameters;
            _optionType = optionType;
            S = spot;
            K = strike;
            T = timeToMaturity;
            r = riskFreeRate;
            σ = volatility;
            λ = _jumpParameters.λ;
            μJ = _jumpParameters.μJ;
            σJ = _jumpParameters.σJ;
            b = r;
        }

        public Func<int, double> σ_ => (n) => Sqrt(σ*σ + n* σJ*σJ/T);
        public Func<int, double> r_ => (n) => r - λ * κ + n * (μJ + σJ*σJ/2)/T;
        public double λ_ => λ * (1 + κ);
        public double κ => Exp(μJ + 0.5 * σJ * σJ) - 1;
        public double Premium() {
            double sum = 0;
            for (int n = 0; n < 40; n++) {
                double factor = Exp(-λ_ * T) * Pow(λ_ * T, n) / Fact(n);
                double bsPrice = new BlackScholes(_optionType, S, K, T, r_(n), σ_(n)).Premium;
                sum += factor * bsPrice;
            }
            return sum;
        }
    }
}
