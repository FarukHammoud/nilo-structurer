using Domain;

namespace Application {
    public class CashFlow : INonPathDependentContract {
        private List<Tuple<DateTime, double>> _cashFlows;

        public CashFlow(List<Tuple<DateTime, double>> cashFlows) {
            _cashFlows = cashFlows;
        }

        public List<DateTime> Dates => _cashFlows.Select(e => e.Item1).ToList();
        public List<Double> Values => _cashFlows.Select(e => e.Item2).ToList();
        public required Currency Currency { get; set; } // ignored for the moment
        public List<Tuple<DateTime, INonPathDependentPayoff>> Payoffs =>
            _cashFlows.Select(e => Tuple.Create(e.Item1, (INonPathDependentPayoff) new DeterministicPayoff(e.Item2))).ToList();
        public double Notional { get; set; } = 1.0;
    }
}
