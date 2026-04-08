namespace Domain {
    public abstract class PathDependentContract : Contract {
        public abstract IPathDependentPayoff Payoff { get; }
        public required DateTime Maturity { get; set; }
    }
}
