using Domain;

namespace Application {
    public class CashFlows : IPathIndependentContract {
        public IEnumerable<CashFlow> Flows { get; }

        public CashFlows(IEnumerable<CashFlow> cashFlows) {
            Flows = cashFlows;
        }

        public IEnumerable<DateTime> Dates => Flows.Select(e => e.PaymentDate);
        public IEnumerable<double> Values => Flows.Select(e => e.Amount);
        public required Currency Currency { get; set; } // ignored for the moment
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs =>
            Flows.Select(e => new DeterministicPayoff() { 
                Maturity = e.PaymentDate,
                PaymentDate = e.PaymentDate,
                PayoffValue = e.Amount,
                Currency = Currency});
        public double Notional { get; set; } = 1.0;
    }
}
