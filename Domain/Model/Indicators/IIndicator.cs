namespace Domain {
    public interface IIndicator {
        /// <summary>
        /// Returns bumped scenarios needed to calculate the indicator. The market data passed in is the unshifted market data, so the indicator can decide which shifts are needed based on that.
        /// </summary>
        /// <param name="marketData">The unshifted market data.</param>
        /// <param name="pricingDate">The date for which the pricing is being performed.</param>
        /// <returns>A list of shifted market data scenarios.</returns>
        IList<(IMarketData, DateTime)> GetShiftedMarketData(IMarketData marketData, DateTime pricingDate);
        /// <summary>
        /// Calculates the result of the indicator based on the unshifted market data and the results of the shifted scenarios.
        /// </summary>
        /// <param name="unshiftedMarketData">The unshifted market data.</param>
        /// <param name="resultsByShift">A dictionary mapping shifted market data scenarios to their corresponding results.</param>
        /// <returns>The calculated result of the indicator.</returns>
        IIndicatorResult GetResult(IMarketData unshiftedMarketData, DateTime pricingDate, Dictionary<(IMarketData, DateTime), ValueWithPrecision> resultsByShift);
    }
}
