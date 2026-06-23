using Domain;
namespace Application {
    public class ZeroCouponBond : IPathIndependentContract {

        public CashFlows CashFlows => new CashFlows([new CashFlow {
            PaymentDate = Maturity,
            Amount = Notional,
            Currency = this.Currency
        }]) { Currency = this.Currency };
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs => CashFlows.PathIndependentPayoffs;

        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public double Notional { get; set; } = 1.0;
    }
}
