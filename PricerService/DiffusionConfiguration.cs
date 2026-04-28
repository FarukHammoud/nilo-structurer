using BrownianServices;
using Domain;

namespace PricerServices {
    public class DiffusionConfiguration {
        required public IMarketData MarketData { get; set; }
        required public List<DateTime> TimeDiscretization { get; set; }
        required public int NumberOfDrawings = 1000;

        public BrowniansConfiguration BrowniansConfiguration =>
            new BrowniansConfiguration {
                Underlyings = MarketData.GetUnderlyings(),
                CorrelationMatrix = MarketData.GetCorrelationMatrix(MarketData.GetUnderlyings()),
                NumberOfDrawings = NumberOfDrawings,
                NumberOfSteps = TimeDiscretization.Count
            };
    }
}
