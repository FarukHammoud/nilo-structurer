namespace Domain {
    public interface INonPathDependentPayoff : IPayoff {
        double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity);
        IEnumerable<Underlying> Dependencies { get; }
        Currency Currency { get; } 

        public double GetUnderlyingValue(Underlying underlying, Dictionary<Underlying, double> pricesAtMaturity) {
            if (underlying is StructuredUnderlying structuredUnderlying) {
                return structuredUnderlying.GetValue(pricesAtMaturity);
            }
            return pricesAtMaturity[underlying];
        }
    }
}
