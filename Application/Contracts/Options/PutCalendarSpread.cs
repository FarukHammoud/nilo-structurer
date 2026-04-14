using Domain;

namespace Application {
    public class PutCalendarSpread : Package {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required DateTime Maturity1 { get; set; }
        public required DateTime Maturity2 { get; set; }
        public override List<INonPathDependentContract> Contracts => new() {
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity1, Strike = Strike, Notional = Notional },
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity2, Strike = Strike, Notional = Notional}};
    }
}
