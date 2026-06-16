using Domain;

namespace Application {
    // TODO : Link with solver utils
    public class FiniteDifferencePdeSolver : IPathIndependentPricer, IPathDependentPricer {
        private IMarketData _marketData;
        private List<DateTime> _timeDiscretization;
        private FiniteDifferenceBsPdeSolver _finiteDifferenceBsPdeSolver;

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            _marketData = marketData;
            _timeDiscretization = timeDiscretization;
            _finiteDifferenceBsPdeSolver = new FiniteDifferenceBsPdeSolver();
        }

        public PriceWithPrecision PricePayoff(IPathIndependentPayoff payoff, DateTime today, Currency pricingCurrency) {
            // TODO: Convert payoff in a payoff by entry, or change signature
            // _finiteDifferenceBsPdeSolver.PriceEuropean()
            throw new NotImplementedException();
        }

        public PriceWithPrecision PricePayoff(IPathDependentPayoff payoff, DateTime today, Currency pricingCurrency) {
            // TODO: Convert payoff in a payoff by entry, or change signature
            // _finiteDifferenceBsPdeSolver.PriceAmerican()
            throw new NotImplementedException();
        }
    }
}
