using Domain;

namespace Application {
    public abstract class KnockOutPayoff : IPathDependentPayoff {
        private readonly IPathDependentPayoff _basePayoff;
        public double Level { get; set; }
        public double Rebate { get; set; }
        public Underlying Underlying { get; set; }
        public abstract Func<Dictionary<DateTime, double>, bool> IsTouched { get; }
        public KnockOutPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying, double rebate = 0) {
            _basePayoff = basePayoff;
            Level = level;
            Underlying = underlying;
            Rebate = rebate;
        }

        public List<DateTime> GetObservationDates() {
            return _basePayoff.GetObservationDates();
        }

        public double GetPayoffAtMaturity(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> barrierUnderlyingPrices = prices.Pivot()[Underlying];
            if (IsTouched(barrierUnderlyingPrices)) {
                return Rebate;
            }
            return _basePayoff.GetPayoffAtMaturity(prices);
        }

        public IReadOnlyList<Underlying> GetUnderlyingDependencyList() {
            return _basePayoff.GetUnderlyingDependencyList().Append(Underlying).ToList();
        }

        public MonitoringFrequency GetMonitoringFrequency() {
            return MonitoringFrequency.Continuous;
        }
    }
}
