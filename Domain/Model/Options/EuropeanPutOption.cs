using Domain.Model;
using Domain.Model.Payoffs;

namespace Domain {
    public class EuropeanPutOption : PutOption, INonPathDependentPayoff {
        public IMonoUnderlyingNonPathDependentPayoff Payoff { get; set; }
        public EuropeanPutOption() {
            Payoff = new StatelessPayoff(
            spot => Math.Max(0, Strike - spot));
        }

        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return Payoff.GetPayoff(pricesAtMaturity[Underlying]);
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return new List<Underlying>() { Underlying };
        }

        public double GetPayoff(double priceAtMaturity) {
            return Payoff.GetPayoff(priceAtMaturity);
        }
    }
}
