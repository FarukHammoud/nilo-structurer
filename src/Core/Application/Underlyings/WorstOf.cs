using Domain;

namespace Application {
    public record WorstOf : StructuredUnderlying {
        public List<Underlying> Underlyings { get; set; }
        public override Currency Currency { get; }
        public WorstOf(List<Underlying> underlyings, Currency currency)
            : base("WorstOf_" + string.Join("_", underlyings.Select(u => u.Code))) {
            Underlyings = underlyings;
            Currency = currency;
        }

        public override double GetValue(Dictionary<Underlying, double> prices) {
            return Underlyings.Min(u => prices[u]);
        }

        public override IEnumerable<Underlying> Dependencies =>
            Underlyings.SelectMany(u => u.Dependencies)
                    .Distinct();      
    }
}
