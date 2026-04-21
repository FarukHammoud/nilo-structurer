using Application;
using Domain;
using MathNet.Numerics.Statistics;

namespace PricerServices.Pricers {
    public class NonPathDependentDiffusionPricer : INonPathDependentPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private DiffusionResult? _diffusion;
        
        private Func<IMarketData, List<DateTime>, DiffusionConfiguration> _diffusionConfigurationFactory = (marketData, timeDiscretization) => new DiffusionConfiguration {
            NumberOfDrawings = 50000,
            CorrelationMatrix = marketData.GetCorrelationMatrix(marketData.GetUnderlyings()),
            Underlyings = marketData.GetUnderlyings(),
            Drifts = marketData.GetUnderlyings().ToDictionary(x => x, x => marketData.GetDrift(x)),
            Spots = marketData.GetUnderlyings().ToDictionary(x => x, x => marketData.GetSpot(x)),
            TimeDiscretization = timeDiscretization,
            Volatilities = marketData.GetUnderlyings().ToDictionary(x => x, x => marketData.GetVolatility(x))
        };

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization) {
            _diffusionConfiguration = _diffusionConfigurationFactory(marketData, timeDiscretization);
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_diffusionConfiguration);
        }

        public ValueWithPrecision Price(INonPathDependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today) {
            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }
            Dictionary<Underlying, List<double>> lastResults = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path[path.Length - 1]).ToList());
            double[] payoffsAtMaturity = new double[_diffusionConfiguration.NumberOfDrawings];
            for (int event_id = 0; event_id < _diffusionConfiguration.NumberOfDrawings; event_id++) {
                Dictionary<Underlying, double> priceAtMaturity = lastResults.ToDictionary(entry => entry.Key, entry => entry.Value[event_id]);
                payoffsAtMaturity[event_id] = payoff.GetPayoffAtMaturity(priceAtMaturity);
            }
            return new ValueWithPrecision() {
                Value = discounter.GetDiscountFactor(maturity, today) * payoffsAtMaturity.Average(),
                Precision = payoffsAtMaturity.StandardDeviation() / Math.Sqrt(_diffusionConfiguration.NumberOfDrawings)
            };
        }
    }
}
