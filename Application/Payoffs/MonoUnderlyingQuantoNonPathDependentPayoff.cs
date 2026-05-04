using Domain;

namespace Application {
    public class MonoUnderlyingQuantoNonPathDependentPayoff : INonPathDependentPayoff {
        private readonly Func<double, double> _payoffMap;
        private readonly Underlying _underlying;
        private readonly Currency _currency;
        private readonly CurrencyPair _currencyPair;
        private readonly double _fxRate;

        public MonoUnderlyingQuantoNonPathDependentPayoff(Func<double, double> payoffMap, Underlying underlying, Currency currency, double fxRate) {
            _payoffMap = payoffMap;
            _underlying = underlying;
            _currency = currency;
            _currencyPair = new CurrencyPair(underlying.Currency, currency);
            _fxRate = fxRate;
        }

        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {    
            double underlyingValue = ((INonPathDependentPayoff)this).GetUnderlyingValue(_underlying, pricesAtMaturity);
            double Xt = 1 / pricesAtMaturity[CurrencyPairs.EURUSD];
            double radonNikodymDerivative = Xt / _fxRate;
            return _fxRate * radonNikodymDerivative * _payoffMap(underlyingValue);
        }

        public IEnumerable<Underlying> Dependencies => _underlying.Dependencies.Append(_currencyPair);
        public Currency Currency => _currency;
    }
}
