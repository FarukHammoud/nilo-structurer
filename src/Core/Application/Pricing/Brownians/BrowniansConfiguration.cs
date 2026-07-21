using Domain;

namespace Application {
    public class BrowniansConfiguration : IBrowniansConfiguration {
        public required IList<Underlying> Underlyings { get; set; }
        public required double[,] CorrelationMatrix { get; set; }
        public int NumberOfDrawings { get; set; } = 1000;
        public required int NumberOfSteps { get; set; }
    }
}
