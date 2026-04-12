using Domain;

namespace Application {
    public class Strangle : Package {

        public required Underlying Underlying { get; set; }
        public required double Strike1 { get; set; }
        public required double Strike2 { get; set; }
        public override List<NonPathDependentContract> Contracts => new () {
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity, Strike = Strike2, Notional = Notional },
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike1, Notional = Notional}};
    }
}
