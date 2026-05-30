namespace Domain {
    public abstract class SinglePayoffPathIndependentContract : IPathIndependentContract {
        public abstract IPathIndependentPayoff Payoff { get; }
        public required Currency Currency { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; } = 1.0;

        public IEnumerable<IPathIndependentPayoff> Payoffs => [Payoff];

        public IEnumerable<DateTime> Dates => [Maturity];
    }
}
