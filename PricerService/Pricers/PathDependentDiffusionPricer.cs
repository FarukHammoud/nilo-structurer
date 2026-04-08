using Application;
using Domain;
using MathNet.Numerics.Statistics;

namespace PricerServices.Pricers {
    public class PathDependentDiffusionPricer : IMultiUnderlyingPricer<IPathDependentPayoff, IMarketData> {

        public ValueWithPrecision Price(IPathDependentPayoff payoff, IMarketData marketData, DateTime maturity, DateTime today) {
            List<Underlying> underlyings = payoff.GetUnderlyingDependencyList();
            // Maybe a constructor based on market data ?
            DiffusionConfiguration diffusionConfiguration = new() {
                NumberOfDrawings = 50000,
                CorrelationMatrix = marketData.GetCorrelationMatrix(underlyings),
                Underlyings = underlyings,
                Drifts = underlyings.ToDictionary(x => x, x => marketData.GetDrift(x)),
                Spots = underlyings.ToDictionary(x => x, x => marketData.GetSpot(x)),
                TimeDiscretization = Enumerable.Range(0, (int)(maturity - today).TotalDays + 1)
                    .Select(i => today.AddDays(i))
                    .ToList(),
                Volatilities = underlyings.ToDictionary(x => x, x => marketData.GetVolatility(x))
            };
            DiffusionResult diffusion = GeneralDiffusion.DiffuseMultiUnderlying(diffusionConfiguration);
            Dictionary<Underlying, List<double>> lastResults = diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path[path.Length - 1]).ToList());
            double[] payoffsAtMaturity = new double[diffusion.NumberOfEvents];
            for (int event_id = 0; event_id < diffusion.NumberOfEvents; event_id++) {
                Dictionary<Underlying, double> priceAtMaturity = lastResults.ToDictionary(entry => entry.Key, entry => entry.Value[event_id]);
                payoffsAtMaturity[event_id] = payoff.GetPayoffAtMaturity(priceAtMaturity);
            }
            return new ValueWithPrecision() {
                Value = marketData.GetDiscountFactor(maturity, today) * payoffsAtMaturity.Average(),
                Precision = payoffsAtMaturity.StandardDeviation() / Math.Sqrt(diffusion.NumberOfEvents)
            };
        }
    }
}
