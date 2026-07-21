using Domain;

namespace Application {
    public class DiffusionPricer : PayoffPricer, IPricer {

        private IDiffusionConfiguration? _configuration;
        private Diffusion? _diffusion;
        
        private Func<IMarketData, IList<DateTime>, IDiffusionConfiguration> _diffusionConfigurationFactory = (marketData, timeDiscretization) => new DiffusionConfiguration {
            MarketData         = marketData,
            TimeDiscretization = timeDiscretization,
            Currency           = Currencies.USD,
        };

        public override void Initialize(IMarketData marketData, IList<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            base.Initialize(marketData, timeDiscretization, pricerConfiguration);
            if (pricerConfiguration is DiffusionPricerConfiguration diffusionConfiguration) {
                _configuration = new DiffusionConfiguration {
                    NumberOfDrawings   = diffusionConfiguration.NumberOfDrawings,
                    MarketData         = marketData,
                    TimeDiscretization = timeDiscretization,
                    Currency           = diffusionConfiguration.Currency,
                    WithControlVariate = diffusionConfiguration.WithControlVariate,
                    HasStochasticRate  = diffusionConfiguration.HasStochasticRate
                };
            } else {
                _configuration = _diffusionConfigurationFactory(marketData, timeDiscretization);
            }
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_configuration);
        }

        public override PriceWithPrecision PricePayoff(IPayoff payoff, DateTime today, Currency pricingCurrency) {
            
            if (_diffusion == null || _configuration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }

            IEnumerable<DateTime> datesOfInterest = payoff.MonitoringFrequency == MonitoringFrequency.Continuous ?
                _configuration.TimeDiscretization : payoff.ObservationDates;
            Dictionary<DateTime, Dictionary<Underlying, List<double>>> pricesAtDiscretizationPoints = new();
 
            foreach (DateTime date in datesOfInterest) {
                pricesAtDiscretizationPoints[date] = _diffusion[date];
            }

            double[] prices        = new double[_configuration.NumberOfDrawings];
            for (int ω = 0; ω < _configuration.NumberOfDrawings; ω++) {
                Scenario scenario = new Scenario(pricesAtDiscretizationPoints.ToDictionary(entry => entry.Key, entry => entry.Value.ToDictionary(e => e.Key, e => e.Value[ω])));
                prices[ω] = payoff.ComputePayoff(scenario);
            }
      
            List<double> discountedPayoffs = new();
            IDictionary<Currency, ShortRate> shortRates = _configuration.Underlyings.OfType<ShortRate>().ToDictionary(x => x.Currency, x => x);
            if (shortRates.ContainsKey(pricingCurrency)) {
                ShortRate shortRate = shortRates[pricingCurrency];
                IList<DateTime> dates = _configuration.TimeDiscretization;
                for (int ω = 0; ω < prices.Length; ω++) {
                    double payoffValue = prices[ω];
                    SimulatedPath shortRatePath = _diffusion[shortRate][ω];
                    ShortRateDiscounter discounter = new ShortRateDiscounter(shortRatePath, dates);
                    double stochasticDF = discounter.GetDiscountFactor(payoff.PaymentDate, today);
                    discountedPayoffs.Add(stochasticDF * payoffValue);
                }
            } else {
                IDiscounter discounter = _configuration.MarketData.GetDiscounter(pricingCurrency);
                double discountFactor  = discounter.GetDiscountFactor(payoff.PaymentDate, today);
                discountedPayoffs = prices.Select(payoffValue => discountFactor * payoffValue).ToList();
            }
             
            if (_configuration.WithControlVariate && payoff is IPathIndependentPayoff) {
                Dictionary<Underlying, List<double>> lastResults = _diffusion.Lasts();

                List<List<double>> controlVariates   = lastResults.Select(entry => entry.Value).ToList();
                Dictionary<Underlying, double> spots = _diffusion.Underlyings.ToDictionary(udl => udl, udl => _diffusion[udl].Paths[0][0]);
                List<double> expectations            = lastResults.Keys.Select(underlying => spots[underlying] / new UnderlyingDiscounterProvider(underlying, _configuration.Currency, _configuration.MarketData).GetDiscountFactor(payoff.PaymentDate, today)).ToList();
                List<double> realizedAverages        = lastResults.Values.Select(values => values.Average()).ToList(); // debugging purposes

                IVarianceReducer varianceReducer = new ControlVariateReducer(controlVariates, expectations, prices.ToList());
                discountedPayoffs = varianceReducer.Adjust(discountedPayoffs);
            }

            return new PriceWithPrecision(discountedPayoffs, payoff.Currency);
        }
    }

}
