using Domain;

namespace Application {
    public class ShiftedEquityMarketData : ShiftedUnderlyingMarketData, IEquityMarketData, IShiftedUnderlyingMarketData {
        public ShiftedEquityMarketData(IUnderlyingMarketData base_) : base(base_) { }

        public double GetCarry() {
            double bump = _shifts.OfType<CarryShift>().Sum(shift => shift.Bump);
            return GetDividend() + GetRepo() + bump;
        }

        public double GetDividend() {
            if (_inner is not IEquityMarketData equityMarketData) {
                throw new InvalidOperationException("Inner market data does not support equity-specific data");
            } // TODO : This is a bit of a code smell. We should consider refactoring to avoid this type check, perhaps by having a more specific base class or interface for equity market data that includes the dividend and repo methods. For now, we will proceed with this approach.
            double bump = _shifts.OfType<DividendShift>().Sum(shift => shift.Bump);
            return equityMarketData.GetDividend() + bump;
        }

        public double GetRepo() {
            if (_inner is not IEquityMarketData equityMarketData) {
                throw new InvalidOperationException("Inner market data does not support equity-specific data");
            }
            double bump = _shifts.OfType<RepoShift>().Sum(shift => shift.Bump);
            return equityMarketData.GetRepo() + bump;
        }
  
    }
}
