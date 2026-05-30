using Application;
using Domain;

namespace PricerServices {
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

        public PriceWithPrecision Price(IPathIndependentPayoff payoff, IDiscounter discounter, IFxConverter fxConverter, DateTime maturity, DateTime today, Currency pricingCurrency) {
            // TODO: Convert payoff in a payoff by entry, or change signature
            // _finiteDifferenceBsPdeSolver.PriceEuropean()
            throw new NotImplementedException();
        }

        public PriceWithPrecision Price(IPathDependentPayoff payoff, IDiscounter discounter, IFxConverter fxConverter, DateTime maturity, DateTime today, Currency pricingCurrency) {
            // TODO: Convert payoff in a payoff by entry, or change signature
            // _finiteDifferenceBsPdeSolver.PriceAmerican()
            throw new NotImplementedException();
        }
    }
}
