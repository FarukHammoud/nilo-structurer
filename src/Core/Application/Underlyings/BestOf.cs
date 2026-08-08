using Domain;

namespace Application {
    public record BestOf : StructuredUnderlying {
        public List<Underlying> Underlyings { get; set; }
        public override Currency Currency { get; }
        public BestOf(List<Underlying> underlyings, Currency currency)
            : base("BestOf_" + string.Join("_", underlyings.Select(u => u.Code))) {
            Underlyings = underlyings;
            Currency = currency;
        }

        public override double GetValue(Dictionary<Underlying, double> prices) {
            return Underlyings.Max(u => prices[u]);
        }

        public override IEnumerable<Underlying> Dependencies =>
            Underlyings.SelectMany(u => u.Dependencies)
                    .Distinct();
    }
}
