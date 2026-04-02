namespace Domain {
    public class BinaryPut : Contract, INonPathDependentPayoff {
        public required Underlying Underlying { get; set; }
        public required DateTime Maturity { get; set; }
        public required double Strike { get; set; }

        public double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity) {
            return pricesAtMaturity[Underlying] < Strike ? 1 : 0;
        }

        public List<Underlying> GetUnderlyingDependencyList() {
            return new List<Underlying>() { Underlying };
        }
    }
}
