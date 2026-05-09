namespace Domain {
    // Gatheral, J.,
    // A parsimonious arbitrage-free implied volatility parameterization
    // with application to the valuation of volatility derivatives,
    // Presentation at Global Derivatives, 2004. 
    public class Gatheral : ILocalVolatilityModel {
        private double a; // vertical translation
        private double b; // wings slope
        private double ρ; // counterclockwise rotation
        private double m; // horizontal translation
        private double σ; // curvature

        public Gatheral(double a, double b, double ρ, double m, double σ) {
            this.a = a; 
            this.b = b; 
            this.ρ = ρ; 
            this.m = m; 
            this.σ = σ; 
            if (!checkConstrains()) {
                throw new ArgumentException("Invalid parameters for Gatheral model");
            }
            if (!checkButterflyArbitrage()) {
                throw new ArgumentException("Butterfly arbitrage condition violated");
            }
        }

        public double getVolatility(double k, double timeToMaturity) {
            // k is log-moneyness, i.e. log(spot/strike)
            return a + b * (ρ * (k - m) + Math.Sqrt((k - m) * (k - m) + σ * σ));
        }

        private bool checkButterflyArbitrage() {
            // 
            return b *(1 + Math.Abs(ρ)) < 4;
        }

        private bool checkConstrains() {
            return (a >= 0) && (b >= 0) && (Math.Abs(ρ) < 1) && (σ > 0);
        }
    }
}
