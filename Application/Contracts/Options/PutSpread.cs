using Domain;

namespace Application {
    public class PutSpread : Package {
        public required Underlying Underlying { get; set; }
        public required double Strike1 { get; set; }
        public required double Strike2 { get; set; }
        public override List<NonPathDependentContract> Contracts => new() {
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike1, Notional = -1 },
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike2}};
    }
}
