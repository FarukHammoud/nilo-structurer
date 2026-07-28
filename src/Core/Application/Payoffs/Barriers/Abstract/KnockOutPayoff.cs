using Domain;

namespace Application {
    public abstract class KnockOutPayoff : IPayoff, IContinuousKnockOutBarrier {
        private readonly IPayoff _basePayoff;
        public double BarrierLevel { get; set; }
        public double Rebate { get; set; }
        public Underlying Underlying { get; set; }
        public DateTime StartDate { get; set; }
        public Currency Currency => _basePayoff.Currency;
        public MonitoringFrequency MonitoringFrequency => MonitoringFrequency.Continuous;
        public KnockOutPayoff(IPayoff basePayoff, double level, Underlying underlying, DateTime startDate, double rebate = 0) {
            _basePayoff = basePayoff;
            BarrierLevel = level;
            Underlying = underlying;
            Rebate = rebate;
            StartDate = startDate;
        }

        public double ComputePayoff(Scenario scenario) {
            return _basePayoff.ComputePayoff(scenario);
        }

        public double GetRedemption(Scenario scenario) {
            return Rebate;
        }

        public IEnumerable<Underlying> Dependencies => _basePayoff.Dependencies.Append(Underlying);

        public IReadOnlyList<DateTime> ObservationDates => TimeGridBuilder.WeeklyGrid(_basePayoff.ObservationDates.Union([StartDate]));

        public DateTime PaymentDate => _basePayoff.PaymentDate;
        public DateTime Maturity => _basePayoff.Maturity;

        public abstract bool IsUp { get; }
    }
}
