using Domain;

namespace Application {
    // TODO : Link with solver utils
    public class FiniteDifferencePdeSolver : PayoffPricer, IPricer {
        private IMarketData _marketData;
        private IList<DateTime> _timeDiscretization;
        private FiniteDifferenceBsPdeSolver _finiteDifferenceBsPdeSolver;

        public override void Initialize(IMarketData marketData, IList<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            base.Initialize(marketData, timeDiscretization, pricerConfiguration);
            _marketData = marketData;
            _timeDiscretization = timeDiscretization;
            _finiteDifferenceBsPdeSolver = new FiniteDifferenceBsPdeSolver();
        }

        public override PriceWithPrecision PricePayoff(IPayoff payoff, DateTime today, Currency pricingCurrency) {
            throw new NotImplementedException();
        }

    }
}
