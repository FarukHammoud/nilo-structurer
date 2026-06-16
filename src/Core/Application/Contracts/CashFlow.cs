using Domain;

namespace Application {
    public class CashFlow : IPathIndependentContract {
        private IEnumerable<Tuple<DateTime, double>> _cashFlows;

        public CashFlow(IEnumerable<Tuple<DateTime, double>> cashFlows) {
            _cashFlows = cashFlows;
        }

        public IEnumerable<DateTime> Dates => _cashFlows.Select(e => e.Item1);
        public IEnumerable<double> Values => _cashFlows.Select(e => e.Item2);
        public required Currency Currency { get; set; } // ignored for the moment
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs =>
            _cashFlows.Select(e => new DeterministicPayoff() { 
                Maturity = e.Item1,
                PaymentDate = e.Item1,
                PayoffValue = e.Item2,
                Currency = Currency});
        public double Notional { get; set; } = 1.0;
    }
}
