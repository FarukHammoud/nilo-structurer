using Domain;

namespace Application {
    public sealed class PricingResults {
        private readonly Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> _results;
        public TimeSpan ComputeTime { get; }
        public PricingResults(Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> raw, TimeSpan computeTime) {
            _results = raw;
            ComputeTime = computeTime;
        } 

        private T Get<T>(IContract contract, IIndicator indicator) where T : class, IIndicatorResult {

            if (!_results.TryGetValue(contract, out var byIndicator))
                throw new KeyNotFoundException($"No results for contract {contract}.");

            if (!byIndicator.TryGetValue(indicator, out var result))
                throw new KeyNotFoundException($"No '{indicator.GetType().Name}' result for contract {contract}.");

            return result as T
                ?? throw new InvalidCastException(
                    $"'{indicator.GetType().Name}' on {contract} is a {result.GetType().Name}, not {typeof(T).Name}.");
        }

        // Convenience wrappers for the common cases

        public Estimate Get(IContract contract, IIndicator indicator)
            => Get<GlobalIndicatorResult>(contract, indicator);

        public Estimate Get(IContract contract, IIndicator indicator, Underlying underlying)
            => Get<ByUnderlyingIndicatorResult>(contract, indicator)[underlying];

        public Estimate Get(IContract contract, IIndicator indicator, Underlying first, Underlying second)
            => Get<ByUnderlyingPairIndicatorResult>(contract, indicator)[(first, second)];

    }
}
