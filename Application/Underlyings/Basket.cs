using Domain;

namespace Application {
    public class Basket : StructuredUnderlying {
        public List<Underlying> Underlyings => _components.Select(c => c.Item1).ToList();
        public List<double> Weights => _components.Select(c => c.Item2).ToList();
        private List<Tuple<Underlying, double>> _components;
        public Basket(List<Tuple<Underlying, double>> components, String name)
            : base(name) {
            _components = components;
        }

        public override double GetValue(Dictionary<Underlying, double> prices) {
            return _components.Sum(c => c.Item2 * prices[c.Item1]);
        }

        public override List<Underlying> GetUnderlyingDependencyList() {
            return _components.Select(c => c.Item1)
                .SelectMany(u => u.GetUnderlyingDependencyList())
                .Distinct()
                .ToList();
        }
    }
}
