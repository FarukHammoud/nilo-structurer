using Domain;
namespace Application {
    public class Bond : IPathIndependentContract {

        public CashFlow CashFlows => new CashFlow(GetCashFlows()) { Currency = this.Currency };
        public IEnumerable<IPathIndependentPayoff> Payoffs => CashFlows.Payoffs;


        public double Coupon { get; set; } = 0;
        public required DateTime StartDate { get; set; }
        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public double Notional { get; set; } = 1.0;
        public Func<DateTime, DateTime> NextSchedule { get; set; } = date => date.AddMonths(6); // default semi-annual schedule

        private IEnumerable<Tuple<DateTime, double>> GetCashFlows() {
            List<Tuple<DateTime, double>> cashFlows = new List<Tuple<DateTime, double>>();
            DateTime currentDate = NextSchedule(StartDate);
            while (currentDate < Maturity) {
                cashFlows.Add(Tuple.Create(currentDate, Coupon * Notional));
                currentDate = NextSchedule(currentDate);
            }
            cashFlows.Add(Tuple.Create(Maturity, (1 + Coupon) * Notional)); // final payment with notional
            return cashFlows;
        }
    }
}
