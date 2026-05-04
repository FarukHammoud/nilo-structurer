using Domain;

namespace Application {
    public record WorstOf : StructuredUnderlying {
        public Underlying FirstUnderlying { get; set; }
        public Underlying SecondUnderlying { get; set; }
        public override Currency Currency { get; }
        public WorstOf(Underlying firstUnderlying, Underlying secondUnderlying, Currency currency)
            : base("WorstOf_" + firstUnderlying.Code + "_" + secondUnderlying.Code) {
            FirstUnderlying = firstUnderlying;
            SecondUnderlying = secondUnderlying;
            Currency = currency;
        }

        public override double GetValue(Dictionary<Underlying, double> prices) {
            return Math.Min(prices[FirstUnderlying], prices[SecondUnderlying]);
        }

        public override IEnumerable<Underlying> Dependencies =>
            FirstUnderlying.Dependencies
                    .Union(SecondUnderlying.Dependencies)
                    .Distinct();      
    }
}
