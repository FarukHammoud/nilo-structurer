using Domain;
namespace Application {
    public class DiffusionConfiguration {
        required public IMarketData MarketData { get; set; }
        required public List<DateTime> TimeDiscretization { get; set; }
        required public Currency Currency { get; set; }
        public int NumberOfDrawings = 50000;
        public bool WithControlVariate = true;
        public JumpParameters? JumpParameters { get; set; }

        public BrowniansConfiguration BrowniansConfiguration =>
            new BrowniansConfiguration {
                Underlyings = MarketData.Underlyings,
                CorrelationMatrix = CorrelationMatrixBuilder.GetCorrelationMatrix(MarketData),
                NumberOfDrawings = NumberOfDrawings,
                NumberOfSteps = TimeDiscretization.Count
            };
    }
}
