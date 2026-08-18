namespace Domain {
    public class Heston {
        public double λ { get; } // speed of reversion
        public double v_ { get; } // long-term mean
        public double η { get; } // volatility of volatility
        public Heston(double λ, double v_, double η) {
            this.λ = λ;
            this.v_ = v_;
            this.η = η;
        }
    }
}
