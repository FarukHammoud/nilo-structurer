using Domain;

namespace Application {
    // A swaption is an option to enter into a swap contract at a future date.
    public class Swaption : IContract {
        public double Notional { get; init; }
        public required Swap Swap { get; init; }
        public required double Strike { get; init; }
        public required DateTime Expiry { get; init; }
        public IEnumerable<IFlow> Flows => new List<IFlow> {
            new ExercisableFlow() {
                Payoff = new DeterministicPayoff() {
                    PayoffValue = 0,
                    Currency = Swap.Currency,
                    Maturity = Expiry,
                    PaymentDate = Expiry,
            },
            Date = Expiry,
            ExerciseParty = ExerciseParty.Holder
            } 
        }
        .Union(Swap.GetFlows())
            .ToList();

        public IEnumerable<DateTime> Dates => Swap.Dates.Union([Expiry]).Order();
    }
}
