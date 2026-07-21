using Domain;
namespace Application {
    public class DiffusionConfiguration : IDiffusionConfiguration {
        required public IMarketData MarketData { get; set; }
        required public IList<DateTime> TimeDiscretization { get; set; }
        required public Currency Currency { get; set; }
        public int NumberOfDrawings { get; set; } = 50000;
        public bool WithControlVariate { get; set; } = true;
        public bool HasStochasticRate { get; set; } = false;
        public INumericalScheme NumericalScheme { get; set; } = new LogEulerScheme();
        public IList<Underlying> Underlyings => MarketData.Underlyings;

        public double[,] CorrelationMatrix => CorrelationMatrixBuilder.GetCorrelationMatrix(MarketData);

        public int NumberOfSteps => TimeDiscretization.Count;
    }
}
