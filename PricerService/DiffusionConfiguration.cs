using BrownianServices;
using Domain;

namespace PricerServices {
    public class DiffusionConfiguration {
        required public List<Underlying> Underlyings { get; set; }
        required public Dictionary<Underlying, Double> Spots { get; set; }
        required public Dictionary<Underlying, Double> Drifts { get; set; }
        required public Dictionary<Underlying, ILocalVolatilityModel> Volatilities { get; set; }
        required public List<DateTime> TimeDiscretization { get; set; }
        required public Double[,] CorrelationMatrix;
        required public int NumberOfDrawings = 1000;
    }
}
