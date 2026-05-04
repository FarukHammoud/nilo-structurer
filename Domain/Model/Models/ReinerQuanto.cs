namespace Domain {
    // Eric Reiner Quanto Mechanics paper (1992) formula for pricing Quanto European options,
    // which are European options on an underlying asset denominated
    // in a different currency than the option's payoff currency. 
    public class ReinerQuanto {

        private readonly BlackScholes _blackScholes;
        public double FxRate { get; }
        /// <summary>
        /// Sensitivity to the quanto adjustment term (ρ·σ_S·σ_FX).
        /// Not a standard Greek but useful for hedging correlation risk.
        /// </summary>
        public double QuantoAdjustment { get; }

        public double Premium => FxRate * _blackScholes.Premium;
        public double Delta => FxRate * _blackScholes.Delta;   // ∂V/∂S in domestic ccy
        public double Gamma => FxRate * _blackScholes.Gamma;
        public double Vega => FxRate * _blackScholes.Vega;
        public double Theta => FxRate * _blackScholes.Theta;
        public double QuantoForward => FxRate * _blackScholes.Forward;

        public ReinerQuanto(OptionType optionType, double spot, double strike, double timeToMaturity, double fxRate, double domesticRate, double foreignRate, double volatility, double fxVolatility, double correlation) {
            FxRate = fxRate;
            double quantoAdj = correlation * volatility * fxVolatility;
            QuantoAdjustment = quantoAdj;

            double b = foreignRate - quantoAdj;

            _blackScholes = new BlackScholes(
               optionType,
               spot,
               strike,
               timeToMaturity,
               domesticRate,   // r = r_d for discounting
               volatility,
               costOfCarry: b  // b = r_f - ρ·σ_S·σ_FX
           );
        }

        public double DigitalCallPrice() => FxRate * _blackScholes.DigitalCallPrice();
        public double DigitalPutPrice() => FxRate * _blackScholes.DigitalPutPrice();
    }
}
