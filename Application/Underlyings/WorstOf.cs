using Domain;

namespace Application {
    public record WorstOf : StructuredUnderlying {
        public Underlying FirstUnderlying { get; set; }
        public Underlying SecondUnderlying { get; set; }
        public WorstOf(Underlying firstUnderlying, Underlying secondUnderlying) 
            : base("BestOf_" + firstUnderlying.Code + "_" + secondUnderlying.Code) {
            FirstUnderlying = firstUnderlying;
            SecondUnderlying = secondUnderlying;
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
