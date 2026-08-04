namespace Domain {
    /// <summary>
    /// Black 76 model for pricing options on futures contracts.
    /// Futures already embody the cost of carry, 
    /// so we can use the Black-Scholes formula with the futures price as the spot price,
    /// with the cost of carry set to zero.
    /// </summary>
    public class Black76 {

        private readonly BlackScholes _bs;
        public double Theta => _bs.Theta;
        public double Rho => _bs.Rho;
        public double Gamma => _bs.Gamma;
        public double Delta => _bs.Delta;
        public double Premium => _bs.Premium;
        public double Vega => _bs.Vega;
        public double Forward => _bs.Forward;

        public Black76(OptionType optionType, double futurePrice, double strike, double timeToMaturity, double riskFreeRate, double volatility) {
            _bs = new BlackScholes(optionType, futurePrice, strike, timeToMaturity, riskFreeRate, volatility, 0);
        }
    }
}
