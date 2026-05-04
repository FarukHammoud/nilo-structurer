using Domain;

namespace Application {
    public record BestOf : StructuredUnderlying {
        public Underlying FirstUnderlying { get; set; }
        public Underlying SecondUnderlying { get; set; }
        public BestOf(Underlying firstUnderlying, Underlying secondUnderlying, Currency currency)
            : base("BestOf_" + firstUnderlying.Code + "_" + secondUnderlying.Code) {
            FirstUnderlying = firstUnderlying;
            SecondUnderlying = secondUnderlying;
        }

        public override double GetValue(Dictionary<Underlying, double> prices) {
            return Math.Max(prices[FirstUnderlying], prices[SecondUnderlying]);
        }

        public override IEnumerable<Underlying> Dependencies =>
            FirstUnderlying.Dependencies
                    .Union(SecondUnderlying.Dependencies)
                    .Distinct();

        public override Currency Currency { get; }
    }
}
