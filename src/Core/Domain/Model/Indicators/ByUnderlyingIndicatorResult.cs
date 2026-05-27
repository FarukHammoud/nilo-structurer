namespace Domain {
    public class ByUnderlyingIndicatorResult : IIndicatorResult {
        public Dictionary<Underlying, ValueWithPrecision> Result { get; } = new();
    }
}
