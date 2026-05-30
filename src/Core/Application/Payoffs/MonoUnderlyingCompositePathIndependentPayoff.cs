using Domain;

namespace Application {
    public class MonoUnderlyingCompositePathIndependentPayoff : IPathIndependentPayoff {
        public required Func<double, double> Payoff { get; init; }
        public required Underlying Underlying { get; init; }
        public required Currency Currency { get; init; }
        public required DateTime Maturity { get; init; }
        public required DateTime PaymentDate { get; init; }
        public CurrencyPair CurrencyPair => new CurrencyPair(Underlying.Currency, Currency);

        public double ComputePayoff(Dictionary<Underlying, double> pricesAtMaturity) {    
            double underlyingValue = Underlying.GetValue(pricesAtMaturity);
            double fxValue = pricesAtMaturity[CurrencyPair];
            return fxValue * Payoff(underlyingValue);
        }

        public IEnumerable<Underlying> Dependencies => Underlying.Dependencies.Append(CurrencyPair);
        
    }
}
