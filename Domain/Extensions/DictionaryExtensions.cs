namespace Domain {
    public static class DictionaryExtensions {
        public static Dictionary<TKey2, Dictionary<TKey1, TResult>> Pivot<TKey1, TKey2, TResult>(
    this Dictionary<TKey1, Dictionary<TKey2, TResult>> source) {
            return source
                .SelectMany(outer => outer.Value.Select(inner => (outer.Key, inner.Key, inner.Value)))
                .GroupBy(x => x.Item2)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(x => x.Item1, x => x.Item3)
                );
        }
        public static int GetContentHashCode<TKey, TValue>(this Dictionary<TKey, TValue> dict) {
            var hash = new HashCode();
            foreach (var (key, value) in dict.OrderBy(x => x.Key?.GetHashCode())) {
                hash.Add(key);
                hash.Add(value);
            }
            return hash.ToHashCode();
        }
    }
}
