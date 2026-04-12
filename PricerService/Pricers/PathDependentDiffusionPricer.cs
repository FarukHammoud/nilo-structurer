using Application;
using Domain;
using MathNet.Numerics.Statistics;

namespace PricerServices.Pricers {
    public class PathDependentDiffusionPricer : IMultiUnderlyingPathDependentPricer<IPathDependentPayoff, IMarketData> {

        public ValueWithPrecision Price(IPathDependentPayoff payoff, IMarketData marketData, DateTime maturity, DateTime today) {
            List<Underlying> underlyings = payoff.GetUnderlyingDependencyList();
            List<DateTime> datesOfInterest = payoff.GetDatesOfInterest();
            List<DateTime> timeDiscretization = Enumerable.Range(0, (int)(maturity - today).TotalDays + 1)
                .Select(i => today.AddDays(i))
                .Union(datesOfInterest)
                .ToList();
            DiffusionConfiguration diffusionConfiguration = getDiffusionConfiguration(marketData, underlyings, timeDiscretization);
            DiffusionResult diffusion = GeneralDiffusion.DiffuseMultiUnderlying(diffusionConfiguration);
            Dictionary<DateTime, Dictionary<Underlying, List<double>>> pricesAtDiscretizationPoints = new();
            foreach (DateTime date in datesOfInterest) {
                int index = timeDiscretization.IndexOf(date);
                pricesAtDiscretizationPoints[date] = diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path[index]).ToList());
            }
            double[] payoffsAtMaturity = new double[diffusion.NumberOfEvents];
            for (int event_id = 0; event_id < diffusion.NumberOfEvents; event_id++) {
                Dictionary<DateTime, Dictionary<Underlying, double>> pricesAtInterestDates = pricesAtDiscretizationPoints.ToDictionary(entry => entry.Key, entry => entry.Value.ToDictionary(e => e.Key, e => e.Value[event_id]));
                payoffsAtMaturity[event_id] = payoff.GetPayoffAtMaturity(pricesAtInterestDates);
            }
            return new ValueWithPrecision() {
                Value = marketData.GetDiscountFactor(maturity, today) * payoffsAtMaturity.Average(),
                Precision = payoffsAtMaturity.StandardDeviation() / Math.Sqrt(diffusion.NumberOfEvents)
            };
        }

        public DiffusionConfiguration getDiffusionConfiguration(IMarketData marketData, List<Underlying> underlyings, List<DateTime> timeDiscretization) {
            return new DiffusionConfiguration() {
                NumberOfDrawings = 50000,
                CorrelationMatrix = marketData.GetCorrelationMatrix(underlyings),
                Underlyings = underlyings,
                Drifts = underlyings.ToDictionary(x => x, marketData.GetDrift),
                Spots = underlyings.ToDictionary(x => x, marketData.GetSpot),
                TimeDiscretization = timeDiscretization,
                Volatilities = underlyings.ToDictionary(x => x, marketData.GetVolatility)
            };
        }
    }
}
