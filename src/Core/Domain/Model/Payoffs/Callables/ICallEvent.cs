namespace Domain {
    public interface ICallEvent {
            DateTime Date { get; }
            Func<Dictionary<Underlying, double>, bool> IsTriggered { get; }
            IPathDependentPayoff Redemption { get; }
    }
}
