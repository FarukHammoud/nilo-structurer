using Domain;

namespace Application{

    // To be deprecated in favor of the contract implementing the IKnockInContract itself
    public abstract class KnockInPayoff : IPathDependentPayoff {
        private readonly IPathDependentPayoff _basePayoff;
        public double Level { get; set; }
        public Underlying Underlying { get; set; }
        public Currency Currency => _basePayoff.Currency;
        public MonitoringFrequency MonitoringFrequency => MonitoringFrequency.Continuous;
        public abstract Func<Dictionary<DateTime, double>, bool> IsTouched { get; }
        public KnockInPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying) {
            _basePayoff = basePayoff;
            Level = level;
            Underlying = underlying;
        }


        public double ComputePayoff(Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> barrierUnderlyingPrices = prices.Pivot()[Underlying];
            if (IsTouched(barrierUnderlyingPrices)) {
                return _basePayoff.ComputePayoff(prices);
            }
            return 0;
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies.Append(Underlying);

        public IReadOnlyList<DateTime> ObservationDates => _basePayoff.ObservationDates;

        public DateTime PaymentDate => _basePayoff.PaymentDate;
    }
}
