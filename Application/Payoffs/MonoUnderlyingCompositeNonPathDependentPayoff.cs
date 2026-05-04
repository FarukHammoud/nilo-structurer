using Domain;

namespace Application {
    public class MonoUnderlyingCompositeNonPathDependentPayoff : INonPathDependentPayoff {
        private readonly Func<double, double> _payoffMap;
        private readonly Underlying _underlying;
        private readonly Currency _currency;
        private readonly CurrencyPair _currencyPair;

        public MonoUnderlyingCompositeNonPathDependentPayoff(Func<double, double> payoffMap, Underlying underlying, Currency currency) {
            _payoffMap = payoffMap;
            _underlying = underlying;
            _currency = currency;
            _currencyPair = new CurrencyPair(underlying.Currency, currency);
        }

        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {    
            double underlyingValue = ((INonPathDependentPayoff)this).GetUnderlyingValue(_underlying, pricesAtMaturity);
            double fxValue = 1 / pricesAtMaturity[CurrencyPairs.EURUSD];
            return fxValue * _payoffMap(underlyingValue);
        }

        public IEnumerable<Underlying> Dependencies => _underlying.Dependencies.Append(_currencyPair);
        public Currency Currency => _currency;
    }
}
