using Domain;

namespace Application {
    /// <summary>
    /// Base class for pricers that can only price independent payoffs.
    /// </summary>
    public abstract class PayoffPricer : IPricer {
        private IMarketData? _marketData;
        virtual public void Initialize(IMarketData marketData, IList<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            _marketData = marketData;
        }
        public abstract PriceWithPrecision PricePayoff(IPayoff payoff, DateTime today, Currency pricingCurrency);
        public PriceWithPrecision Price(IContract contract, DateTime today, Currency pricingCurrency) {
            double price = 0.0, precisionSquared = 0.0;
            foreach (IFlow flow in contract.Flows) {
                if (flow is not IPayoff payoff) {
                    throw new InvalidOperationException($"Flow {flow} is not a payoff.");
                }
                PriceWithPrecision payoffPv = PricePayoff(payoff, today, pricingCurrency);
                double fxRate               = _marketData.GetFxRate(payoffPv.Currency, pricingCurrency);
                price += payoffPv.Value * fxRate;
                precisionSquared += Math.Pow(payoffPv.Precision * fxRate, 2);
            }
            return new PriceWithPrecision() {
                Value = price,
                Precision = Math.Sqrt(precisionSquared),
                Currency = pricingCurrency
            };
        }
    }
}