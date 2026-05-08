using Domain;

namespace Application {
    public class DiffusionPricerConfiguration : IPricerConfiguration {
        public int NumberOfDrawings { get; init; }
        public required Currency Currency { get; init; }
    }
}
