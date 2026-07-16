namespace Domain {
    /// <summary>
    /// Represents an automatic end of a product when a certain condition is met.
    /// </summary>
    public interface IAutoCallFlow : IFlow {
        bool IsTriggered(Scenario scenario);
        IPayoff Rebate { get; }
        DateTime IFlow.Date => Rebate.Date;
    }
}
