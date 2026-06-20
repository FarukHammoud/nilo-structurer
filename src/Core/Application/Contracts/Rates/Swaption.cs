using Domain;

namespace Application {
    // A swaption is an option to enter into a swap contract at a future date.
    // We need a way to get the swap price, same as done with structured underlying
    public class Swaption : IPathDependentContract {
        public double Notional { get; init; }
        public required Swap Swap { get; init; }
        public required double Strike { get; init; }
        public required DateTime Expiry { get; init; }
        public IEnumerable<IPathDependentPayoff> PathDependentPayoffs => Swap.PathIndependentPayoffs;
    }
}
