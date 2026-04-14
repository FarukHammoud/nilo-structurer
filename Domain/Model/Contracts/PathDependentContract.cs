namespace Domain {
    public abstract class PathDependentContract : IContract {
        public abstract IPathDependentPayoff Payoff { get; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; } = 1;
    }
}
