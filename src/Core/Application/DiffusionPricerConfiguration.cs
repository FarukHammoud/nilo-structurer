using Domain;

namespace Application {
    public class DiffusionPricerConfiguration : IPricerConfiguration {
        public int NumberOfDrawings { get; init; } = 50000;
        public required Currency Currency { get; init; }
        public bool WithControlVariate { get; init; } = true;
    }
}
