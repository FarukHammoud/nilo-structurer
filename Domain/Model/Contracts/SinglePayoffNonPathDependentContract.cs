namespace Domain {
    public abstract class SinglePayoffNonPathDependentContract : INonPathDependentContract {
        public abstract INonPathDependentPayoff Payoff { get; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; } = 1.0;

        public List<Tuple<DateTime, INonPathDependentPayoff>> Payoffs => [Tuple.Create(Maturity, Payoff)];
    }
}
