using Application;
using Domain;
using MathNet.Numerics.Statistics;

namespace PricerServices.Pricers {
    public class PathDependentDiffusionPricer : IPathDependentPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private DiffusionResult? _diffusion;

        public  PriceWithPrecision Price(
            IPathDependentPayoff payoff, 
            IDiscounter discounter, 
            IFxConverter fxConverter, 
            DateTime maturity, 
            DateTime today,
            Currency pricingCurrency) {
            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }
            List<DateTime> datesOfInterest = payoff.GetObservationDates();
            Dictionary<DateTime, Dictionary<Underlying, List<double>>> pricesAtDiscretizationPoints = new();
            if (payoff.GetMonitoringFrequency() == MonitoringFrequency.Continuous) {
                datesOfInterest = _diffusionConfiguration.TimeDiscretization;
            }
            foreach (DateTime date in datesOfInterest) {
                int index = _diffusionConfiguration.TimeDiscretization.IndexOf(date);
                pricesAtDiscretizationPoints[date] = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path[index]).ToList());
            }
            double[] payoffsAtMaturity = new double[_diffusion.NumberOfEvents];
            for (int event_id = 0; event_id < _diffusion.NumberOfEvents; event_id++) {
                Dictionary<DateTime, Dictionary<Underlying, double>> pricesAtInterestDates = pricesAtDiscretizationPoints.ToDictionary(entry => entry.Key, entry => entry.Value.ToDictionary(e => e.Key, e => e.Value[event_id]));
                payoffsAtMaturity[event_id] = payoff.GetPayoffAtMaturity(pricesAtInterestDates);
            }
            return new PriceWithPrecision() {
                Value = discounter.GetDiscountFactor(maturity, today) * payoffsAtMaturity.Average(),
                Precision = payoffsAtMaturity.StandardDeviation() / Math.Sqrt(_diffusion.NumberOfEvents),
                Currency = payoff.Currency  
            };
        }

        public DiffusionConfiguration getDiffusionConfiguration(IMarketData marketData, List<DateTime> timeDiscretization) {
            List<Underlying> underlyings = marketData.GetUnderlyings();
            return new DiffusionConfiguration() {
                NumberOfDrawings = 50000,
                MarketData = marketData,
                TimeDiscretization = timeDiscretization
            };
        }

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization) {
            //List<DateTime> datesOfInterest = payoff.GetDatesOfInterest();
            //List<DateTime> timeDiscretization = Enumerable.Range(0, (int)(maturity - today).TotalDays + 1)
            //    .Select(i => today.AddDays(i))
            //    .Union(datesOfInterest)
            //    .ToList();
            _diffusionConfiguration = getDiffusionConfiguration(marketData, timeDiscretization);
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_diffusionConfiguration);
        }
    }
}
