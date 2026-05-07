using Domain;

namespace Application {
    public interface IShiftedUnderlyingMarketData : IUnderlyingMarketData {
        IUnderlyingMarketData WithShift(UnderlyingShift shift);
    }
}
