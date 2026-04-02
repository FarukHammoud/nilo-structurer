using Domain.Model;
using Domain.Model.Payoffs;

namespace Domain {
    public class EuropeanCallOption : CallOption, INonPathDependentPayoff {
        public IMonoUnderlyingNonPathDependentPayoff Payoff { get; set; }
        public EuropeanCallOption() {
            Payoff = new StatelessPayoff(
            spot => Math.Max(0, spot - Strike));
        }

        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return Payoff.GetPayoff(pricesAtMaturity[Underlying]);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return new List<Underlying>() { Underlying };
        }
    }
}
