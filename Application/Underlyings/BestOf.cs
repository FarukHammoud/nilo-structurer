using Domain;

namespace Application {
    public class BestOf : StructuredUnderlying {
        public Underlying FirstUnderlying { get; set; }
        public Underlying SecondUnderlying { get; set; }
        public BestOf(Underlying firstUnderlying, Underlying secondUnderlying) 
            : base("BestOf_" + firstUnderlying.Code + "_" + secondUnderlying.Code) {
            FirstUnderlying = firstUnderlying;
            SecondUnderlying = secondUnderlying;
        }

        public override double GetValue(Dictionary<Underlying, double> prices) {
            return Math.Max(prices[FirstUnderlying], prices[SecondUnderlying]);
        }

        public override List<Underlying> GetUnderlyingDependencyList() {
            return FirstUnderlying.GetUnderlyingDependencyList()
                .Union(SecondUnderlying.GetUnderlyingDependencyList())
                .ToList();
        }
    }
}
