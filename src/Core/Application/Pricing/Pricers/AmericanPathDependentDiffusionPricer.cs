using Domain;
using System.Runtime.CompilerServices;

namespace Application {
    public class AmericanPathDependentDiffusionPricer : IPathDependentPricer {

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
            List<DateTime> datesOfInterest = payoff.ObservationDates.ToList();
            Underlying underlying = _diffusion.DiffusionValues.Keys.First();
            Dictionary<DateTime, Dictionary<Underlying, List<double>>> pricesAtDiscretizationPoints = new();
            if (payoff.MonitoringFrequency == MonitoringFrequency.Continuous) {
                datesOfInterest = _diffusionConfiguration.TimeDiscretization;
            }
            LongstaffSchwartzAmericanSimulation longstaffSchwartzPricer = new LongstaffSchwartzAmericanSimulation();
            Func<double, double> payoffMap = spot => payoff.ComputePayoff(new Dictionary<DateTime, Dictionary<Underlying, double>> {
                { maturity, new Dictionary<Underlying, double> { [underlying] = spot} } });
            ValueWithPrecision price = longstaffSchwartzPricer.PriceAmerican(
                payoffMap,
                today,
                datesOfInterest,
                _diffusion.DiffusionValues[underlying], 
                discounter);
            return new PriceWithPrecision() {
                Value = price.Value,
                Precision = price.Precision,
                Currency = payoff.Currency  
            };
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
