using Domain;

namespace Application {
    public class Strangle : SinglePayoffPackage {

        public required Underlying Underlying { get; set; }
        public required double Strike1 { get; set; }
        public required double Strike2 { get; set; }
        public required DateTime Maturity { get; set; }
        public override List<SinglePayoffNonPathDependentContract> SinglePayoffContracts => new () {
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity, Strike = Strike2, Notional = Notional },
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike1, Notional = Notional}};
    }
}
