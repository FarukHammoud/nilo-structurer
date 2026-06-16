using Domain;

namespace Application {
    public class PathIndependentDiffusionPricer : IPathIndependentPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private DiffusionResult? _diffusion;
        
        private Func<IMarketData, List<DateTime>, DiffusionConfiguration> _diffusionConfigurationFactory = (marketData, timeDiscretization) => new DiffusionConfiguration {
            MarketData         = marketData,
            TimeDiscretization = timeDiscretization,
            Currency           = Currencies.USD,
        };

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            if (pricerConfiguration is DiffusionPricerConfiguration diffusionPricerConfiguration) {
                _diffusionConfiguration = new DiffusionConfiguration {
                    NumberOfDrawings   = diffusionPricerConfiguration.NumberOfDrawings,
                    MarketData         = marketData,
                    TimeDiscretization = timeDiscretization,
                    Currency           = diffusionPricerConfiguration.Currency,
                    WithControlVariate = diffusionPricerConfiguration.WithControlVariate,
                };
            } else {
                _diffusionConfiguration = _diffusionConfigurationFactory(marketData, timeDiscretization);
            }
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_diffusionConfiguration);
        }

        public PriceWithPrecision PricePayoff(IPathIndependentPayoff payoff, DateTime today, Currency pricingCurrency) {
            
            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }
            
            Dictionary<Underlying, List<double>> lastResults = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths.Select(path => path.Last).ToList());
            double[] payoffsAtMaturity = new double[_diffusionConfiguration.NumberOfDrawings];
            for (int ω = 0; ω < _diffusionConfiguration.NumberOfDrawings; ω++) {
                Dictionary<Underlying, double> priceAtMaturity = lastResults.ToDictionary(entry => entry.Key, entry => entry.Value[ω]);
                payoffsAtMaturity[ω] = payoff.ComputePayoff(priceAtMaturity);
            }
            List<double> discountedPayoffs = new();
            IDictionary<Currency, ShortRate> shortRates = _diffusionConfiguration.MarketData.Underlyings.OfType<ShortRate>().ToDictionary(x => x.Currency, x => x);
            if (shortRates.ContainsKey(pricingCurrency)) {
                ShortRate shortRate = shortRates[pricingCurrency];
                double t = (payoff.PaymentDate - today).TotalYears;
                for (int i = 0; i < payoffsAtMaturity.Length; i++) {
                    double rate = lastResults[shortRate][i];
                    double payoffValue = payoffsAtMaturity[i];
                    // single step is probably not enough, 
                    // df is integral of short rate
                    // need to map stochastic rates configuration into a path dependent pricer
                    double discountFactor = Math.Exp(-rate * t);
                    discountedPayoffs.Add(discountFactor * payoffValue);
                }
            } else {
                IDiscounter discounter         = _diffusionConfiguration.MarketData.GetDiscounter(pricingCurrency);
                double discountFactor          = discounter.GetDiscountFactor(payoff.PaymentDate, today);
                discountedPayoffs = payoffsAtMaturity.Select(payoffValue => discountFactor * payoffValue).ToList();
            }
             

            if (_diffusionConfiguration.WithControlVariate) {
                List<List<double>> controlVariates   = lastResults.Select(entry => entry.Value).ToList();
                Dictionary<Underlying, double> spots = _diffusion.DiffusionValues.ToDictionary(x => x.Key, x => x.Value.Paths[0][0]);
                List<double> expectations            = lastResults.Keys.Select(underlying => spots[underlying] / new UnderlyingDiscounterProvider(underlying, _diffusionConfiguration.Currency, _diffusionConfiguration.MarketData).GetDiscountFactor(payoff.PaymentDate, today)).ToList();
                List<double> realizedAverages        = lastResults.Values.Select(values => values.Average()).ToList(); // debugging purposes

                IVarianceReducer varianceReducer = new ControlVariateReducer(controlVariates, expectations, payoffsAtMaturity.ToList());
                discountedPayoffs = varianceReducer.Adjust(discountedPayoffs);
            }
            return new PriceWithPrecision(discountedPayoffs, payoff.Currency);
        }

    }
}
