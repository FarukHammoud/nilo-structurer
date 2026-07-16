using Domain;

namespace Application {
    public class AutoCall : IContract {
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Coupon { get; set; }
        public required double ProtectionBarrier { get; set; }
        public required double InitialPrice { get; set; }
        public required List<DateTime> ObservationDates { get; set; }
        public DateTime Maturity => ObservationDates.Last();
        public double Notional { get; set; } = 1.0;

        private IPayoff FinalPayoff =>
            new MonoUnderlyingPathIndependentPayoff() {
                Payoff = (priceAtMaturity) => {
                    if (priceAtMaturity < ProtectionBarrier) {
                        return Notional * priceAtMaturity / InitialPrice;

                    } else if (priceAtMaturity < InitialPrice) {
                        return Notional;
                    } else {
                        return Notional * (1 + Coupon * (ObservationDates.Count));
                    }
                },
                PaymentDate = Maturity,
                Maturity = Maturity,
                Underlying = Underlying,
                Currency = Currency
            };

        private List<IAutoCallFlow> AutoCallEvents => 
            Enumerable.Range(0, ObservationDates.Count - 1)
                .Select(i => (IAutoCallFlow) new AutoCallFlow() { 
                    Date = ObservationDates[i],
                    TriggerMap = prices => prices[Underlying][i] > InitialPrice,
                    Rebate = new MonoUnderlyingPathIndependentPayoff() {
                        Payoff = (priceAtMaturity) => Notional * (1 + i * Coupon),
                        PaymentDate = ObservationDates[i],
                        Maturity = ObservationDates[i],
                        Underlying = Underlying,
                        Currency = Currency,
                    }
            }).ToList();

        public IEnumerable<DateTime> Dates => AutoCallEvents.Select(p => p.Date).Distinct();

        public IEnumerable<IFlow> Flows => AutoCallEvents.Union([(IFlow)FinalPayoff]);
    }
}
