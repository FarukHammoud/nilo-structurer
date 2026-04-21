namespace Domain {
    public interface IIndicator {
        /// <summary>
        /// Returns bumped scenarios needed to calculate the indicator. The market data passed in is the unshifted market data, so the indicator can decide which shifts are needed based on that.
        /// </summary>
        /// <param name="marketData">The unshifted market data.</param>
        /// <returns>A list of shifted market data scenarios.</returns>
        IList<IMarketData> GetShiftedMarketData(IMarketData marketData);
        /// <summary>
        /// Calculates the result of the indicator based on the unshifted market data and the results of the shifted scenarios.
        /// </summary>
        /// <param name="unshiftedMarketData">The unshifted market data.</param>
        /// <param name="resultsByShift">A dictionary mapping shifted market data scenarios to their corresponding results.</param>
        /// <returns>The calculated result of the indicator.</returns>
        ValueWithPrecision GetResult(IMarketData unshiftedMarketData, Dictionary<IMarketData, ValueWithPrecision> resultsByShift);
    }
}
