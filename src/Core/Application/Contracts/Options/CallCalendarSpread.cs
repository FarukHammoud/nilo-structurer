using Domain;

namespace Application {
    public class CallCalendarSpread : SinglePayoffPackage {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required DateTime Maturity1 { get; set; }
        public required DateTime Maturity2 { get; set; }
        public required Currency Currency { get; set; }
        public override List<SinglePayoffNonPathDependentContract> SinglePayoffContracts => new() {
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity1, Strike = Strike, Notional = Notional, Currency = Currency },
            new EuropeanCall() { Underlying = Underlying, Maturity = Maturity2, Strike = Strike, Notional = Notional, Currency = Currency }};
    
    }
}
