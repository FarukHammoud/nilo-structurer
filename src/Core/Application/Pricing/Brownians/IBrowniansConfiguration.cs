using Domain;

namespace Application {
    public interface IBrowniansConfiguration {
        IList<Underlying> Underlyings { get; }
        double[,] CorrelationMatrix { get; }
        int NumberOfDrawings { get; }
        int NumberOfSteps { get; }
    }
}
