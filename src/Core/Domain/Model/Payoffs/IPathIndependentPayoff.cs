namespace Domain {
    /// <summary>
    /// Base contract for any path-independent payoff.
    /// </summary>
    public interface IPathIndependentPayoff : IPayoff {
        double GetPayoffAtMaturity(Dictionary<Underlying, double> pricesAtMaturity);
    }
}
