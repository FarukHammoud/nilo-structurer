using Domain;

namespace Application {
    public class PutSpread : Package {
        public required Underlying Underlying { get; set; }
        public required double Strike1 { get; set; }
        public required double Strike2 { get; set; }
        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public override List<INonPathDependentContract> Contracts => new() {
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike1, Notional = -1, Currency = Currency },
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike2, Currency = Currency }};
    }
}
