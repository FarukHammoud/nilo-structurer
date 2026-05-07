using Domain;

namespace Application {
    public class ShiftedCurrencyPairMarketData : ShiftedUnderlyingMarketData, ICurrencyPairMarketData, IShiftedUnderlyingMarketData {
        public ShiftedCurrencyPairMarketData(IUnderlyingMarketData base_) : base(base_) { }
    }
}
