namespace Domain {
    public class ByUnderlyingPairIndicatorResult : IIndicatorResult {
        private Dictionary<(Underlying First, Underlying Second), Estimate> _result = new();
        public Estimate this[(Underlying First, Underlying Second) pair] {
            get => _result[pair];
            set {
                _result[pair] = value;
            }
        }
    }
}
