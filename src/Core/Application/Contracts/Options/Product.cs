using Domain;

namespace Application.Contracts.Options {
    public class Product : IPathIndependentContract {
        public required string Name { get; set; }
        public required DateTime Maturity { get; set; }
        public required Currency Currency { get; set; }
        public required List<SinglePayoffPathIndependentContract> Contracts { get; set; }

        public IEnumerable<IPathIndependentPayoff> Payoffs => [new ProductPathIndependentPayoff(Contracts.Select(c => c.Payoff), Currency)];

        public double Notional => Contracts.Aggregate(1.0, (product, contract) => product * contract.Notional);
    }
}
