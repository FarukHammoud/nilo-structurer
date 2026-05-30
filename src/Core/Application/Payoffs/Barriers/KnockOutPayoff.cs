using Domain;

namespace Application {
    public abstract class KnockOutPayoff : IPathDependentPayoff {
        private readonly IPathDependentPayoff _basePayoff;
        public double Level { get; set; }
        public double Rebate { get; set; }
        public Underlying Underlying { get; set; }
        public Currency Currency => _basePayoff.Currency;
        public MonitoringFrequency MonitoringFrequency => MonitoringFrequency.Continuous;
        public abstract Func<Dictionary<DateTime, double>, bool> IsTouched { get; }
        public KnockOutPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying, double rebate = 0) {
            _basePayoff = basePayoff;
            Level = level;
            Underlying = underlying;
            Rebate = rebate;
        }

        public double ComputePayoff(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> barrierUnderlyingPrices = prices.Pivot()[Underlying];
            if (IsTouched(barrierUnderlyingPrices)) {
                return Rebate;
            }
            return _basePayoff.ComputePayoff(prices);
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies.Append(Underlying);

        public IReadOnlyList<DateTime> ObservationDates => _basePayoff.ObservationDates;

        public DateTime PaymentDate => _basePayoff.PaymentDate;
    }
}
