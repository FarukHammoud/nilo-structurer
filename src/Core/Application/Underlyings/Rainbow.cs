using Domain;

namespace Application {
    /// <summary>
    /// Weighted Basked ordered by best performances
    /// </summary>
    /// TODO: Refactor GetValue to take Scenario as input in order to compute performance
    public record Rainbow : StructuredUnderlying {
        public IList<Underlying> Underlyings { get; }
        public IList<double> Weights { get; }
        public override Currency Currency { get; }

        public Rainbow(IList<Underlying> underlyings, IList<double> weights, Currency currency)
            : base("Rainbow_" + string.Join("_", underlyings.Select(u => u.Code))) {
            if (weights.Count > underlyings.Count) {
                throw new ArgumentException("More weights than possible underlyings");
            }
            Underlyings = underlyings;
            Weights = weights;
            Currency = currency;
        }

        public override double GetValue(Dictionary<Underlying, double> prices) {
            double sum = 0;
            var sortedPairs = prices.OrderBy(kvp => kvp.Value).ToList();
            for (int i = 0; i< Weights.Count; i++) {
                double price = sortedPairs[i].Value;
                sum += price;
            }
            return sum;
        }

        public override IEnumerable<Underlying> Dependencies =>
            Underlyings.SelectMany(u => u.Dependencies).Distinct();
    }
}
