using Domain;

namespace Application.Contracts.Options {
    public class Product : INonPathDependentContract {
        public required string Name { get; set; }
        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public required List<SinglePayoffNonPathDependentContract> Contracts { get; set; }

        public IEnumerable<Tuple<DateTime, INonPathDependentPayoff>> Payoffs => [Tuple.Create(Maturity, (INonPathDependentPayoff)new ProductNonPathDependentPayoff(Contracts.Select(c => c.Payoff), Currency))];

        public double Notional => Contracts.Aggregate(1.0, (product, contract) => product * contract.Notional);
    }
}
