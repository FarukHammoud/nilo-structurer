using Domain;

namespace Application{
    public abstract class KnockInPayoff : IPathDependentPayoff {
        private readonly IPathDependentPayoff _basePayoff;
        public double Level { get; set; }
        public Underlying Underlying { get; set; }
        public Currency Currency => _basePayoff.Currency;
        public abstract Func<Dictionary<DateTime, double>, bool> IsTouched { get; }
        public KnockInPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying) {
            _basePayoff = basePayoff;
            Level = level;
            Underlying = underlying;
        }

        public List<DateTime> GetObservationDates() {
            return _basePayoff.GetObservationDates();
        }

        public double GetPayoffAtMaturity(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> barrierUnderlyingPrices = prices.Pivot()[Underlying];
            if (IsTouched(barrierUnderlyingPrices)) {
                return _basePayoff.GetPayoffAtMaturity(prices);
            }
            return 0;
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies.Append(Underlying);

        public MonitoringFrequency GetMonitoringFrequency() {
            return MonitoringFrequency.Continuous;
        }
    }
}
