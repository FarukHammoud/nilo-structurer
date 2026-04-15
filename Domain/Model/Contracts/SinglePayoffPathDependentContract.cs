namespace Domain {
    public abstract class SinglePayoffPathDependentContract : IPathDependentContract {
        public abstract IPathDependentPayoff Payoff { get; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; } = 1.0;

        public IEnumerable<Tuple<DateTime, IPathDependentPayoff>> Payoffs => [Tuple.Create(Maturity, Payoff)];
    }
}
