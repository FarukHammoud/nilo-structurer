namespace Domain {
    public interface INonPathDependentPayoff : IPayoff {
        double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity);
        List<Underlying> GetUnderlyingDependencyList();
    }
}
