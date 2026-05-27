namespace Application {
    public abstract class CompositeVanilla : VanillaContract {
        public required double InitialFxRate { get; set; } // should come from market data at term
    }
}
