namespace Domain.Model.Models {

    // Composite option on an equity with FX risk, using Black-Scholes as the underlying model.
    // Priced via change of numeraire to the foreign currency and then applying Black-Scholes with an adjusted volatility.
    public class Composite {

        private readonly BlackScholes _blackScholes;

        public double Premium => _blackScholes.Premium;
        public double Delta => _blackScholes.Delta;
        public double Vega => _blackScholes.Vega;
        public double Theta => _blackScholes.Theta;
        public double Gamma => _blackScholes.Gamma;

        public Composite(
            OptionType optionType,
            double foreignSpot,         
            double foreignStrike,       
            double timeToMaturity,
            double fxRate, // domestic currency per unit of foreign currency        
            double foreignRate,
            double volatility) 
        {

            _blackScholes = new BlackScholes(
                optionType,
                spot: foreignSpot * fxRate,  
                strike: foreignStrike * fxRate,
                timeToMaturity: timeToMaturity,
                riskFreeRate: foreignRate,
                volatility: volatility
            );
        }
    }
}
