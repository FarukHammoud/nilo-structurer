namespace Domain {
    /// <summary>
    /// Base contract for any path-independent payoff.
    /// </summary>
    public interface IPathIndependentPayoff : IPayoff {
        DateTime Maturity { get; init; }
        double ComputePayoff(Dictionary<Underlying, double> pricesAtMaturity);
    }
}
