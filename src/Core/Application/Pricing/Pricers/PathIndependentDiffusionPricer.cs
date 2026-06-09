using Domain;
using MathNet.Numerics.Statistics;

namespace Application {
    public class PathIndependentDiffusionPricer : IPathIndependentPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private DiffusionResult? _diffusion;
        
        private Func<IMarketData, List<DateTime>, DiffusionConfiguration> _diffusionConfigurationFactory = (marketData, timeDiscretization) => new DiffusionConfiguration {
            MarketData = marketData,
            TimeDiscretization = timeDiscretization,
            Currency = Currencies.USD,
        };

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            if (pricerConfiguration is DiffusionPricerConfiguration diffusionPricerConfiguration) {
                _diffusionConfiguration = new DiffusionConfiguration {
                    NumberOfDrawings = diffusionPricerConfiguration.NumberOfDrawings,
                    MarketData = marketData,
                    TimeDiscretization = timeDiscretization,
                    Currency = diffusionPricerConfiguration.Currency,
                    WithControlVariate = diffusionPricerConfiguration.WithControlVariate,
                };
            } else {
                _diffusionConfiguration = _diffusionConfigurationFactory(marketData, timeDiscretization);
            }
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_diffusionConfiguration);
        }

        public PriceWithPrecision Price(IPathIndependentPayoff payoff, IDiscounter discounter, IFxConverter fxConverter, DateTime maturity, DateTime today, Currency pricingCurrency) {
            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }
            Dictionary<Underlying, List<double>> lastResults = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path.Last).ToList());
            double[] payoffsAtMaturity = new double[_diffusionConfiguration.NumberOfDrawings];
            for (int ω = 0; ω < _diffusionConfiguration.NumberOfDrawings; ω++) {
                Dictionary<Underlying, double> priceAtMaturity = lastResults.ToDictionary(entry => entry.Key, entry => entry.Value[ω]);
                payoffsAtMaturity[ω] = payoff.ComputePayoff(priceAtMaturity);
            }
            double discountFactor = discounter.GetDiscountFactor(maturity, today);
            List<double> discountedPayoffs = payoffsAtMaturity.Select(payoffValue => discountFactor * payoffValue).ToList();

            List<List<double>> controlVariates = lastResults.Select(entry => entry.Value).ToList();
            Dictionary<Underlying, double> spots = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths[0][0]);
            List<double> expectations = lastResults.Keys.Select(underlying => spots[underlying] / new UnderlyingDiscounterProvider(underlying, _diffusionConfiguration.Currency, _diffusionConfiguration.MarketData).GetDiscountFactor(maturity, today)).ToList();
            List<double> realizedAverages = lastResults.Values.Select(values => values.Average()).ToList(); // debugging purposes

            IVarianceReducer varianceReducer = new ControlVariateReducer(controlVariates, expectations, payoffsAtMaturity.ToList());
            if (_diffusionConfiguration.WithControlVariate) {
                discountedPayoffs = varianceReducer.Adjust(discountedPayoffs);
            }
            return new PriceWithPrecision(discountedPayoffs, payoff.Currency);
        }

    }
}
