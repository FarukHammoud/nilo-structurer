using Application;
using Domain;
using MathNet.Numerics.Statistics;

namespace PricerServices.Pricers {
    public class GeneralDiffusionPricer : IMultiUnderlyingPricer<INonPathDependentPayoff, IMarketData> {

        public ValueWithPrecision Price(INonPathDependentPayoff payoff, IMarketData marketData, DateTime maturity, DateTime today) {
            // delete this one
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

        public List<ValueWithPrecision> PriceSeveralPayoffs(List<INonPathDependentPayoff> payoffs, IMarketData marketData, List<DateTime> maturities, DateTime today) {
            // rewrite it to be more clear, 
            List<Underlying> underlyings = marketData.GetUnderlyings();
            List<DateTime> timeDiscretization = Enumerable.Range(0, (int)(maturities.Max() - today).TotalDays + 1)
                    .Select(i => today.AddDays(i))
                    .ToList();
            // Maybe a constructor based on market data ?
            DiffusionConfiguration diffusionConfiguration = new() {
                NumberOfDrawings = 50000,
                CorrelationMatrix = marketData.GetCorrelationMatrix(underlyings),
                Underlyings = underlyings,
                Drifts = underlyings.ToDictionary(x => x, x => marketData.GetDrift(x)),
                Spots = underlyings.ToDictionary(x => x, x => marketData.GetSpot(x)),
                TimeDiscretization = timeDiscretization,
                Volatilities = underlyings.ToDictionary(x => x, x => marketData.GetVolatility(x))
            };
            DiffusionResult diffusion = GeneralDiffusion.DiffuseMultiUnderlying(diffusionConfiguration);
            Dictionary<Underlying, List<double>> lastResults = diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path[path.Length - 1]).ToList());
            List<double[]> payoffsAtMaturity = new();// double[diffusion.NumberOfEvents];
            for (int event_id = 0; event_id < diffusion.NumberOfEvents; event_id++) {
                if (event_id == 0) {
                    payoffsAtMaturity = new List<double[]>(payoffs.Count);
                    for (int i = 0; i < payoffs.Count; i++) {
                        payoffsAtMaturity.Add(new double[diffusion.NumberOfEvents]);
                    }
                }
                Dictionary<Underlying, double> priceAtMaturity = lastResults.ToDictionary(entry => entry.Key, entry => entry.Value[event_id]);
                foreach (var i in Enumerable.Range(0, payoffs.Count)) {
                    payoffsAtMaturity[i][event_id] = payoffs[i].GetPayoffAtMaturity(priceAtMaturity);
                }
            }
            return Enumerable.Range(0, payoffs.Count).Select(i => new ValueWithPrecision() {
                Value = marketData.GetDiscountFactor(maturities[i], today) * payoffsAtMaturity[i].Average(),
                Precision = payoffsAtMaturity[i].StandardDeviation() / Math.Sqrt(diffusion.NumberOfEvents)
            }).ToList();
        }
    }
}
