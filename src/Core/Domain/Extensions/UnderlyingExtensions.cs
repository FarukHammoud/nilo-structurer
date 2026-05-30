namespace Domain {
    public static class UnderlyingExtensions {
        public static Dictionary<DateTime, double> GetValues(this Underlying underlying, Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            List<DateTime> dates = prices.Keys.ToList();
            return dates.ToDictionary(
                date => date, 
                date => underlying.GetValue(prices[date]));
        }

        public static double GetValue(this Underlying underlying, Dictionary<Underlying, double> prices) {
            return underlying is StructuredUnderlying structured ? 
                structured.GetValue(prices) : prices[underlying];
        }
    }
}
