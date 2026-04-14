using Domain;

namespace Application {
    public class Straddle : SinglePayoffPackage {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required DateTime Maturity { get; set; }
        public override List<SinglePayoffNonPathDependentContract> SinglePayoffContracts => new () {
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity, Strike = Strike, Notional = Notional },
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike, Notional = Notional}};
    }
}
