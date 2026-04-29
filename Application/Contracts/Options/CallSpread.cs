using Domain;

namespace Application {
    public class CallSpread : Package {

        public required Underlying Underlying { get; set; }
        public required double Strike1 { get; set; }
        public required double Strike2 { get; set; }
        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public override List<INonPathDependentContract> Contracts => new () {
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity, Strike = Strike1, Currency = Currency },
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity, Strike = Strike2, Notional = -1, Currency = Currency }
        };
    }
}
