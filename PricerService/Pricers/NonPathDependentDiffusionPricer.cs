using Application;
using Domain;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.Statistics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PricerServices.Pricers {
    public class NonPathDependentDiffusionPricer : INonPathDependentPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private DiffusionResult? _diffusion;
        
        private Func<IMarketData, List<DateTime>, DiffusionConfiguration> _diffusionConfigurationFactory = (marketData, timeDiscretization) => new DiffusionConfiguration {
            NumberOfDrawings = 50000,
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
                };
            } else {
                _diffusionConfiguration = _diffusionConfigurationFactory(marketData, timeDiscretization);
            }
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_diffusionConfiguration);
        }

        public PriceWithPrecision Price(INonPathDependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today) {
            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }
            Dictionary<Underlying, List<double>> lastResults = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path[path.Length - 1]).ToList());
            double[] payoffsAtMaturity = new double[_diffusionConfiguration.NumberOfDrawings];
            for (int ω = 0; ω < _diffusionConfiguration.NumberOfDrawings; ω++) {
                Dictionary<Underlying, double> priceAtMaturity = lastResults.ToDictionary(entry => entry.Key, entry => entry.Value[ω]);
                payoffsAtMaturity[ω] = payoff.GetPayoffAtMaturity(priceAtMaturity);
            }
            double discountFactor = discounter.GetDiscountFactor(maturity, today);
            List<double> discountedPayoffs = payoffsAtMaturity.Select(payoffValue => discountFactor * payoffValue).ToList();
     
            List<List<double>> controlVariates = lastResults.Select(entry => entry.Value).ToList();
            Dictionary<Underlying, double> spots = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths[0][0]);
            List<double> expectations = lastResults.Keys.Select(underlying => spots[underlying] / discounter.GetDiscountFactor(maturity, today)).ToList();

            IVarianceReducer varianceReducer = new ControlVariateReducer(controlVariates, expectations, payoffsAtMaturity.ToList());
            List<double> adjustedPayoffs = varianceReducer.Adjust(discountedPayoffs);
            
            double price = adjustedPayoffs.Average();
            double precision = adjustedPayoffs.StandardDeviation() / Math.Sqrt(_diffusionConfiguration.NumberOfDrawings);

            return new PriceWithPrecision() {
                Value = price,
                Precision = precision,
                Currency = payoff.Currency
            };
        }

    }
}
