using Domain;
using MathNet.Numerics.Statistics;

namespace Application {
    public class PathDependentDiffusionPricer : IPathDependentPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private DiffusionResult? _diffusion;

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            if (pricerConfiguration is DiffusionPricerConfiguration diffusionPricerConfiguration) {
                _diffusionConfiguration = new DiffusionConfiguration() {
                    NumberOfDrawings = diffusionPricerConfiguration.NumberOfDrawings,
                    MarketData = marketData,
                    TimeDiscretization = timeDiscretization,
                    Currency = Currencies.USD
                };
            } else {
                _diffusionConfiguration = getDiffusionConfiguration(marketData, timeDiscretization);
            }
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_diffusionConfiguration);
        }

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
            IEnumerable<DateTime> datesOfInterest = payoff.ObservationDates;
            Dictionary<DateTime, Dictionary<Underlying, List<double>>> pricesAtDiscretizationPoints = new();
            if (payoff.MonitoringFrequency == MonitoringFrequency.Continuous) {
                datesOfInterest = _diffusionConfiguration.TimeDiscretization;
            }
            foreach (DateTime date in datesOfInterest) {
                int index = _diffusionConfiguration.TimeDiscretization.IndexOf(date);
                pricesAtDiscretizationPoints[date] = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path[index]).ToList());
            }
            double[] prices = new double[_diffusion.NumberOfEvents];
            double DF = discounter.GetDiscountFactor(maturity, today);
            for (int event_id = 0; event_id < _diffusion.NumberOfEvents; event_id++) {
                Dictionary<DateTime, Dictionary<Underlying, double>> pricesAtInterestDates = pricesAtDiscretizationPoints.ToDictionary(entry => entry.Key, entry => entry.Value.ToDictionary(e => e.Key, e => e.Value[event_id]));
                prices[event_id] = DF * payoff.ComputePayoff(pricesAtInterestDates);
            }
            return new PriceWithPrecision(prices, payoff.Currency);
        }

        public DiffusionConfiguration getDiffusionConfiguration(IMarketData marketData, List<DateTime> timeDiscretization) {
            IList<Underlying> underlyings = marketData.Underlyings;
            return new DiffusionConfiguration() {
                NumberOfDrawings = 50000,
                MarketData = marketData,
                TimeDiscretization = timeDiscretization,
                Currency = marketData.Currencies.Contains(Currencies.USD) ? Currencies.USD : marketData.Currencies.First()
            };
        }
    }
}
