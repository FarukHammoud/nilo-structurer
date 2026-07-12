namespace Domain {
    interface IBarrierFlow : IFlow {
        bool IsTriggered(Dictionary<DateTime, double> path);
        IPayoff Payoff { get; }
    }
}
