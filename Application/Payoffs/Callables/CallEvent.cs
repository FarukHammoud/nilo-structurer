using Domain;

namespace Application {
    public class CallEvent : ICallEvent {
        public required DateTime Date { get; set; }
        public required Func<Dictionary<Underlying, double>, bool> IsTriggered { get; set; }
        public required IPathDependentPayoff Redemption { get; set; }   
    }
}
