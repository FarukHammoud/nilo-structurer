using Domain;

namespace BrownianServices {
    public class BrowniansConfiguration {
        public required List<Underlying> Underlyings;
        public required Double[,] CorrelationMatrix;
        public int NumberOfDrawings = 1000;
        public required int NumberOfSteps;
    }
}
