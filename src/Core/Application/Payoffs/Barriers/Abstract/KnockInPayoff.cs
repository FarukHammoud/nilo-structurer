using Domain;

namespace Application{

    // Idea: Keep Decorator composition on payoff creation
    // On pricer it checks IContinuousKnockInBarrier implementation
    public abstract class KnockInPayoff : IPayoff, IContinuousKnockInBarrier {
        private readonly IPayoff _basePayoff;
        public double BarrierLevel { get; set; }
        public Underlying Underlying { get; set; }
        public DateTime StartDate { get; set; }
        public Currency Currency => _basePayoff.Currency;
        public MonitoringFrequency MonitoringFrequency => MonitoringFrequency.Continuous;
        public KnockInPayoff(IPayoff basePayoff, double level, Underlying underlying, DateTime startDate) {
            _basePayoff = basePayoff;
            BarrierLevel = level;
            Underlying = underlying;
            StartDate = startDate;
        }

        public double ComputePayoff(Scenario scenario) {
            return _basePayoff.ComputePayoff(scenario);
        }

        public double GetRedemption(Scenario scenario) {
            return 0;
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies.Append(Underlying);

        public IReadOnlyList<DateTime> ObservationDates => TimeGridBuilder.WeeklyGrid(_basePayoff.ObservationDates.Union([StartDate]));

        public DateTime PaymentDate => _basePayoff.PaymentDate;
        public DateTime Maturity => _basePayoff.Maturity;

        public abstract bool IsUp { get; }
    }
}
