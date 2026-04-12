namespace Domain {
    public interface IPathDependentPayoff : IPayoff {
        double GetPayoffAtMaturity(Dictionary<DateTime, Dictionary<Underlying, double>> prices);
        List<Underlying> GetUnderlyingDependencyList();
        List<DateTime> GetDatesOfInterest();

        public Dictionary<DateTime, double> GetUnderlyingValues(Underlying underlying, Dictionary<DateTime, Dictionary<Underlying, double>> prices) {
            Dictionary<DateTime, double> underlyingValues = new Dictionary<DateTime, double>();
            foreach (DateTime date in prices.Keys) {
                Dictionary<Underlying, double> pricesAtDate = prices[date];
                if (underlying is StructuredUnderlying structuredUnderlying) {
                    underlyingValues[date] = structuredUnderlying.GetValue(pricesAtDate);
                } else {
                    underlyingValues[date] = pricesAtDate[underlying];
                }
            }
            return underlyingValues;
            
        }

    }
}
