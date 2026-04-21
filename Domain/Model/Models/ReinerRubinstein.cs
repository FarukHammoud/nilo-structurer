using MathNet.Numerics.Distributions;

namespace Domain {
    public enum OptionType { Call, Put }
    public enum BarrierDirection { Up, Down }
    public enum BarrierType { KnockIn, KnockOut }

    public class ReinerRubinstein {

        private readonly double _spot;
        private readonly double _strike;
        private readonly double _barrier;
        private readonly double _maturity;
        private readonly double _rate;
        private readonly double _volatility;
        private readonly double _rebate;
        private readonly OptionType _optionType;
        private readonly BarrierDirection _direction;
        private readonly BarrierType _barrierType;

        public ReinerRubinstein(
            double spot, double strike, double barrier, double maturity,
            double rate, double volatility, double rebate,
            OptionType optionType, BarrierDirection direction, BarrierType barrierType) {

            _spot = spot;
            _strike = strike;
            _barrier = barrier;
            _maturity = maturity;
            _rate = rate;
            _volatility = volatility;
            _rebate = rebate;
            _optionType = optionType;
            _direction = direction;
            _barrierType = barrierType;
        }

        public double Price() {
            double ϕ = _optionType == OptionType.Call ? 1 : -1;
            double η = _direction == BarrierDirection.Up ? -1 : 1;  // Haug convention

            double H = _barrier;
            double S = _spot;
            double K = _strike;
            double T = _maturity;
            double r = _rate;
            double b = r;  // cost of carry, assumed to be risk-free rate for simplicity but we can add divs: r - q
            double σ = _volatility;
            double R = _rebate;

            double sqrtT = Math.Sqrt(T);
            double μ = (b - σ * σ / 2) / (σ * σ);
            double λ = Math.Sqrt(μ * μ + 2 * r / (σ * σ));

            double x1 = Math.Log(S / K) / (σ * sqrtT) + (1 + μ) * σ * sqrtT;
            double x2 = Math.Log(S / H) / (σ * sqrtT) + (1 + μ) * σ * sqrtT;

            double y1 = Math.Log(H * H / (S * K)) / (σ * sqrtT) + (1 + μ) * σ * sqrtT;
            double y2 = Math.Log(H / S) / (σ * sqrtT) + (1 + μ) * σ * sqrtT;

            double z = Math.Log(H / S) / (σ * sqrtT) + λ * σ * sqrtT;

            double dfR = Math.Exp(-r * T);
            double dfB = Math.Exp((b - r) * T);

            // Component A
            double A = ϕ * S * dfB * N(ϕ * x1)
              - ϕ * K * dfR * N(ϕ * x1 - ϕ * σ * sqrtT);

            // Component B
            double B = ϕ * S * dfB * N(ϕ * x2)
              - ϕ * K * dfR * N(ϕ * x2 - ϕ * σ * sqrtT);
            // Component C
            double C = ϕ * S * dfB * Math.Pow(H / S, 2.0 * (μ + 1)) * N(η * y1)
              - ϕ * K * dfR * Math.Pow(H / S, 2.0 * μ) * N(η * y1 - η * σ * sqrtT);
            // Component D
            double D = ϕ * S * dfB * Math.Pow(H / S, 2.0 * (μ + 1)) * N(η * y2)
              - ϕ * K * dfR * Math.Pow(H / S, 2.0 * μ) * N(η * y2 - η * σ * sqrtT);

            // Component E
            double E = R * dfR * (N(η * x2 - η * σ * sqrtT)
              - Math.Pow(H / S, 2.0 * μ) * N(η * y2 - η * σ * sqrtT));
            // Component F
            double F = R * (Math.Pow(H / S, μ + λ) * N(η * z)
              + Math.Pow(H / S, μ - λ) * N(η * z - 2.0 * η * λ * σ * sqrtT));
            return (_optionType, _direction, _barrierType) switch {
                // ── CALLS ──────────────────────────────────────────────────────────────────────────
                (OptionType.Call, BarrierDirection.Down, BarrierType.KnockIn) =>
                    (_strike >= _barrier) ? C + F : A - B + D + F,

                (OptionType.Call, BarrierDirection.Down, BarrierType.KnockOut) =>
                    (_strike >= _barrier) ? A - C + E : B - D + E,

                (OptionType.Call, BarrierDirection.Up, BarrierType.KnockIn) =>
                    (_strike >= _barrier) ? A + F : B - C + D + F, // <── FIXED: Use B-C+D for K < H

                (OptionType.Call, BarrierDirection.Up, BarrierType.KnockOut) =>
                    (_strike >= _barrier) ? E : A - B + C - D + E, // <── FIXED: If K > H, KO is just the rebate

                // ── PUTS ───────────────────────────────────────────────────────────────────────────
                (OptionType.Put, BarrierDirection.Down, BarrierType.KnockIn) =>
                    (_strike >= _barrier) ? B - C + D + F : A + F,

                (OptionType.Put, BarrierDirection.Down, BarrierType.KnockOut) =>
                    (_strike >= _barrier) ? A - B + C - D + E : E,

                (OptionType.Put, BarrierDirection.Up, BarrierType.KnockIn) =>
                    (_strike >= _barrier) ? A - B + D + F : C + F,

                (OptionType.Put, BarrierDirection.Up, BarrierType.KnockOut) =>
                    (_strike >= _barrier) ? B - D + E : A - C + E,

                _ => throw new InvalidOperationException("Invalid Barrier Configuration")
            };
        }

        private static double N(double x) => Normal.CDF(0, 1, x);
    }
}