using Domain;

namespace BrownianServices {
    public class BrowniansConfiguration {
        public required IList<Underlying> Underlyings;
        public required Double[,] CorrelationMatrix;
        public int NumberOfDrawings = 1000;
        public required int NumberOfSteps;
    }
}
