namespace Domain {
    public class ByUnderlyingPairIndicatorResult : IIndicatorResult {
        public Dictionary<(Underlying First, Underlying Second), ValueWithPrecision> Result { get; } = new();
    }
}
