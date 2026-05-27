namespace Domain {
    public interface ICallableContract : INonPathDependentContract {
        /// <summary>
        /// Ordered by ObservationDate. Pricer short-circuits on first triggered event.
        /// Unconditional payoffs before the trigger date are still paid.
        /// </summary>
        IEnumerable<ICallEvent> CallEvents { get; }
    }
}
