namespace Domain {
    /// <summary>
    /// Represents a continuous knock-in barrier condition for a financial derivative.
    /// To be added to specific payoffs that have a knock-in feature.
    /// The activated payoff is the payoff itself.
    /// </summary>
    public interface IContinuousKnockInBarrier : IContinuousBarrier;
}
