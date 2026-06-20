using Domain;
namespace Application {
    public class ZeroCouponBond : IPathIndependentContract {

        public CashFlow CashFlows => new CashFlow([Tuple.Create(Maturity, Notional)]) { Currency = this.Currency };
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs => CashFlows.PathIndependentPayoffs;

        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public double Notional { get; set; } = 1.0;
    }
}
