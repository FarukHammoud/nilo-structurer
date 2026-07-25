namespace Domain {
    public class ByUnderlyingIndicatorResult : IIndicatorResult {
        private Dictionary<Underlying, Estimate> _result = new();
        public Estimate this[Underlying underlying] {
            get => _result[underlying];
            set {
                _result[underlying] = value;
            }
        }
    }
}
