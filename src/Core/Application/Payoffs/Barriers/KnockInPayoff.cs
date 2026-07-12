using Domain;

namespace Application{

    // Idea: Keep Decorator composition on payoff creation
    // On pricer it checks IInKnockablePayoff implementation
    public abstract class KnockInPayoff : IPathDependentPayoff, IInKnockablePayoff {
        private readonly IPathDependentPayoff _basePayoff;
        public double Level { get; set; }
        public Underlying Underlying { get; set; }
        public Currency Currency => _basePayoff.Currency;
        public MonitoringFrequency MonitoringFrequency => MonitoringFrequency.Continuous;
        public abstract Func<SimulatedPath, bool> IsTouched { get; }
        public KnockInPayoff(IPathDependentPayoff basePayoff, double level, Underlying underlying) {
            _basePayoff = basePayoff;
            Level = level;
            Underlying = underlying;
        }


        public double ComputePayoff(Scenario scenario) {
            SimulatedPath path = scenario[Underlying];
            if (IsTouched(path)) {
                return _basePayoff.ComputePayoff(scenario);
            }
            return 0;
            // switch to this version an implement barrier on pricer side
            //return _basePayoff.ComputePayoff(prices);
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies.Append(Underlying);

        public IReadOnlyList<DateTime> ObservationDates => _basePayoff.ObservationDates;

        public DateTime PaymentDate => _basePayoff.PaymentDate;
        public DateTime Maturity => _basePayoff.Maturity;

        // Target API for the knock-in condition, to be used by the pricer 
        public IKnockInBarrier KnockInCondition => new SingleUnderlyingKnockInBarrier() {
            ActivatedPayoff = _basePayoff,
            BarrierLevel = Level,
            MonitoringFrequency = MonitoringFrequency.Continuous,
            Underlying = Underlying,
            ObservationDates = ObservationDates,
            IsTouched = IsTouched
        };
    }
}
