namespace Domain {
    /// <summary>
    /// Represents a continuous barrier condition for a financial derivative.
    /// To be added to specific payoffs that have a barrier feature.
    /// The activated payoff is the payoff itself.
    /// </summary>
    public interface IContinuousBarrier {
        bool IsUp { get; }
        double BarrierLevel { get; }
        Underlying Underlying { get; }
    }
}
