namespace Domain {
    public abstract class NonPathDependentContract : Contract {
        public abstract INonPathDependentPayoff Payoff { get; }
        public required DateTime Maturity { get; set; }
    }
}
