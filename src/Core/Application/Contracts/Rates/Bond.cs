using Domain;
namespace Application {
    public class Bond : IPathIndependentContract {

        public CashFlows CashFlows => new CashFlows(GetCashFlows()) { Currency = this.Currency };
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs => CashFlows.PathIndependentPayoffs;


        public double Coupon { get; set; } = 0;
        public required DateTime StartDate { get; set; }
        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public double Notional { get; set; } = 1.0;
        public Func<DateTime, DateTime> NextSchedule { get; set; } = date => date.AddMonths(6); // default semi-annual schedule

        private IEnumerable<CashFlow> GetCashFlows() {
            List<CashFlow> cashFlows = new List<CashFlow>();
            if (Coupon != 0) {
                DateTime currentDate = NextSchedule(StartDate);
                while (currentDate < Maturity) {
                    cashFlows.Add(new CashFlow {
                        PaymentDate = currentDate,
                        Amount = Coupon * Notional,
                        Currency = this.Currency
                    });
                    currentDate = NextSchedule(currentDate);
                }
            }
            cashFlows.Add(new CashFlow {
                PaymentDate = Maturity,
                Amount = (1 + Coupon) * Notional,
                Currency = this.Currency
            });
            return cashFlows;
        }
    }
}
