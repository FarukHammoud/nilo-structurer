namespace Domain {
    public interface IIndicator {
        List<IMarketData> GetShiftedMarketData();
        ValueWithPrecision GetResult(List<ValueWithPrecision> values);
    }
}
