namespace Domain {
    /// <summary>
    /// Represents a continuous knock-out barrier condition for a financial derivative.
    /// To be implemented by specific payoffs that have a knock-out feature.
    /// </summary>
    public interface IContinuousKnockOutBarrier : IContinuousBarrier {
        double GetRedemption(Scenario scenario);
    }
}
