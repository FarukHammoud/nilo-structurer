using Domain;

namespace Application {
    public abstract class KnockOutPayoff : IPathDependentPayoff, IOutKnockablePayoff {
        private readonly IPathDependentPayoff _basePayoff;
        public double Level { get; set; }
        public double Rebate { get; set; }
        public Underlying Underlying { get; set; }
        public Currency Currency => _basePayoff.Currency;
        public MonitoringFrequency MonitoringFrequency => MonitoringFrequency.Continuous;
        public abstract Func<SimulatedPath, bool> IsTouched { get; }
        public KnockOutPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying, double rebate = 0) {
            _basePayoff = basePayoff;
            Level = level;
            Underlying = underlying;
            Rebate = rebate;
        }

        public double ComputePayoff(Scenario scenario) {
            SimulatedPath path = scenario[Underlying];
            if (IsTouched(path)) {
                return Rebate;
            }
            return _basePayoff.ComputePayoff(scenario);
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies.Append(Underlying);

        public IReadOnlyList<DateTime> ObservationDates => _basePayoff.ObservationDates;

        public DateTime PaymentDate => _basePayoff.PaymentDate;
        public DateTime Maturity => _basePayoff.Maturity;

        public IKnockOutBarrier KnockOutCondition => throw new NotImplementedException();
    }
}
