using Domain;

namespace Application {
    public class VarianceDispersion : PathDependentPackage {
        public required List<Underlying> Underlyings { get; set; } = new List<Underlying>();
        public required DateTime StartDate { get; set; }
        public required DateTime Maturity { get; set; }
        public required double VarianceStrike { get; set; }
        public Underlying Index => new Basket(Underlyings.Select(u => Tuple.Create(u, 1.0/Underlyings.Count)).ToList(), "Index") { };
        public override List<IPathDependentContract> Contracts => Underlyings.Select(underlying =>
        (IPathDependentContract) new VarianceSwap() { Underlying = underlying, VarianceStrike = VarianceStrike, StartDate = StartDate, Maturity = Maturity, Notional = Notional }).Append(
            new VarianceSwap() { Underlying = Index, VarianceStrike = VarianceStrike, StartDate = StartDate, Maturity = Maturity, Notional = Notional }).ToList();
    
    }
}
