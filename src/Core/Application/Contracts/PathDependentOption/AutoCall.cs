using Domain;

namespace Application {
    public class AutoCall : IPathDependentContract {
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Coupon { get; set; }
        public required double ProtectionBarrier { get; set; }
        public required double InitialPrice { get; set; }
        public required List<DateTime> ObservationDates { get; set; }
        public DateTime Maturity => ObservationDates.Last();
        public double Notional { get; set; } = 1.0;

        public IEnumerable<Tuple<DateTime, IPathDependentPayoff>> Payoffs =>
            [Tuple.Create(Maturity, (IPathDependentPayoff) new MonoUnderlyingPathDependentPayoff() {
                PayoffMap = (prices) => {
                    double priceAtMaturity = prices[Maturity];
                    if (priceAtMaturity < ProtectionBarrier) {
                        return Notional * priceAtMaturity / InitialPrice;

                    } else if (priceAtMaturity < InitialPrice) {
                        return Notional;
                    } else {
                        return Notional * (1 + Coupon * (ObservationDates.Count));
                    }
                },
                ObservationDates = [Maturity],
                Underlying = Underlying,
                MonitoringFrequency = MonitoringFrequency.None,
                Currency = Currency
            })];

        public IEnumerable<ICallEvent> CallEvents => 
            Enumerable.Range(0, ObservationDates.Count - 1)
                .Select(i => (ICallEvent) new CallEvent() { 
                    Date = ObservationDates[i],
                    IsTriggered = prices => prices[Underlying] > InitialPrice,
                    Redemption = new MonoUnderlyingPathDependentPayoff() {
                        PayoffMap = path => Notional * (1 + i * Coupon),
                        ObservationDates = [ObservationDates[i]],
                        Underlying = Underlying,
                        MonitoringFrequency = MonitoringFrequency.None,
                        Currency = Currency,
                    }
            });
    }
}
