using Domain;

namespace Application {
    public class DiffusionPricer : IPricer<IPayoff>, IPathIndependentPricer, IPathDependentPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private Diffusion? _diffusion;
        
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

        public PriceWithPrecision PricePayoff(IPayoff payoff, DateTime today, Currency pricingCurrency) {
            
            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }

            IEnumerable<DateTime> datesOfInterest = payoff.MonitoringFrequency == MonitoringFrequency.Continuous ?
                _diffusionConfiguration.TimeDiscretization : payoff.ObservationDates;
            Dictionary<DateTime, Dictionary<Underlying, List<double>>> pricesAtDiscretizationPoints = new();
 
            foreach (DateTime date in datesOfInterest) {
                pricesAtDiscretizationPoints[date] = _diffusion[date];
            }

            double[] prices        = new double[_diffusionConfiguration.NumberOfDrawings];
            for (int ω = 0; ω < _diffusionConfiguration.NumberOfDrawings; ω++) {
                Scenario scenario = new Scenario(pricesAtDiscretizationPoints.ToDictionary(entry => entry.Key, entry => entry.Value.ToDictionary(e => e.Key, e => e.Value[ω])));
                prices[ω] = payoff.ComputePayoff(scenario);
            }
      
            List<double> discountedPayoffs = new();
            IDictionary<Currency, ShortRate> shortRates = _diffusionConfiguration.MarketData.Underlyings.OfType<ShortRate>().ToDictionary(x => x.Currency, x => x);
            if (shortRates.ContainsKey(pricingCurrency)) {
                ShortRate shortRate = shortRates[pricingCurrency];
                double t = (payoff.PaymentDate - today).TotalYears;
                List<DateTime> dates = _diffusionConfiguration.TimeDiscretization;
                for (int ω = 0; ω < prices.Length; ω++) {
                    double payoffValue = prices[ω];
                    SimulatedPath shortRatePath = _diffusion[shortRate][ω];
                    double integral = 0;
                    for (int k = 0; k < shortRatePath.Values.Count() - 1; k++) {
                        double dt = (dates[k + 1] - dates[k]).TotalYears;
                        integral += shortRatePath.Values[k] * dt;
                    }
                    double stochasticDF = Math.Exp(-integral);
                    discountedPayoffs.Add(stochasticDF * payoffValue);
                }
            } else {
                IDiscounter discounter = _diffusionConfiguration.MarketData.GetDiscounter(pricingCurrency);
                double discountFactor  = discounter.GetDiscountFactor(payoff.PaymentDate, today);
                discountedPayoffs = prices.Select(payoffValue => discountFactor * payoffValue).ToList();
            }
             
            if (_diffusionConfiguration.WithControlVariate && payoff is IPathIndependentPayoff) {
                Dictionary<Underlying, List<double>> lastResults = _diffusion.Lasts();

                List<List<double>> controlVariates   = lastResults.Select(entry => entry.Value).ToList();
                Dictionary<Underlying, double> spots = _diffusion.Underlyings.ToDictionary(udl => udl, udl => _diffusion[udl].Paths[0][0]);
                List<double> expectations            = lastResults.Keys.Select(underlying => spots[underlying] / new UnderlyingDiscounterProvider(underlying, _diffusionConfiguration.Currency, _diffusionConfiguration.MarketData).GetDiscountFactor(payoff.PaymentDate, today)).ToList();
                List<double> realizedAverages        = lastResults.Values.Select(values => values.Average()).ToList(); // debugging purposes

                IVarianceReducer varianceReducer = new ControlVariateReducer(controlVariates, expectations, prices.ToList());
                discountedPayoffs = varianceReducer.Adjust(discountedPayoffs);
            }

            return new PriceWithPrecision(discountedPayoffs, payoff.Currency);
        }

        public PriceWithPrecision PricePayoff(IPathIndependentPayoff payoff, DateTime today, Currency pricingCurrency) {
            return PricePayoff((IPayoff)payoff, today, pricingCurrency);
        }

        public PriceWithPrecision PricePayoff(IPathDependentPayoff payoff, DateTime today, Currency pricingCurrency) {
            return PricePayoff((IPayoff)payoff, today, pricingCurrency);
        }
    }

}
