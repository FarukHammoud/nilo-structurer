namespace Domain {
    public abstract class VanillaContract {
        public required Underlying Underlying { get; set; }
        public required DateTime Maturity { get; set; }
        public required double Strike { get; set; }
    }
}
