using Domain;

namespace Application {
    public class CashFlow : INonPathDependentContract {
        private IEnumerable<Tuple<DateTime, double>> _cashFlows;

        public CashFlow(IEnumerable<Tuple<DateTime, double>> cashFlows) {
            _cashFlows = cashFlows;
        }

        public IEnumerable<DateTime> Dates => _cashFlows.Select(e => e.Item1);
        public IEnumerable<Double> Values => _cashFlows.Select(e => e.Item2);
        public required Currency Currency { get; set; } // ignored for the moment
        public IEnumerable<Tuple<DateTime, INonPathDependentPayoff>> Payoffs =>
            _cashFlows.Select(e => Tuple.Create(e.Item1, (INonPathDependentPayoff) new DeterministicPayoff(e.Item2, Currency)));
        public double Notional { get; set; } = 1.0;
    }
}
