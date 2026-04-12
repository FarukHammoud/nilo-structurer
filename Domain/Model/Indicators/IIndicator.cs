namespace Domain {
    public interface IIndicator {
        IList<IMarketData> GetShiftedMarketData(IMarketData marketData);
        ValueWithPrecision GetResult(IMarketData unshiftedMarketData, Dictionary<IMarketData, ValueWithPrecision> resultsByShift);
    }
}
