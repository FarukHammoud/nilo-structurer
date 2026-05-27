namespace Domain {
    public static class IEnumerableExtensions {
        public static double Product(this IEnumerable<double> source)
            => source.Aggregate(1.0, (acc, x) => acc * x);
        public static double Product<T>(this IEnumerable<T> source, Func<T, double> selector)
            => source.Aggregate(1.0, (acc, x) => acc * selector(x));
    }
}
