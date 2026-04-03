namespace Domain {
    public class Straddle : Package {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public override List<NonPathDependentContract> Contracts => new () {
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity, Strike = Strike, Notional = Notional },
            new EuropeanPut() { Underlying = Underlying, Maturity = Maturity, Strike = Strike, Notional = Notional}};
    }
}
