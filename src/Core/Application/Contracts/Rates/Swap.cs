using Domain;

namespace Application {
    public class Swap : IPathIndependentContract{
        public IEnumerable<CashFlow> FixedFlows { get; init; }
        public ShortRate FloatingRate { get; init; }

        public Swap(ShortRate floatingRate, double fixedRate, IEnumerable<DateTime> paymentDates) {
            FixedFlows = paymentDates.Select(date => new CashFlow {
                PaymentDate = date,
                Amount = fixedRate,// - floatingRate.GetRate(date),
                Currency = floatingRate.Currency
            });
            FloatingRate = floatingRate;
        }

        public IEnumerable<DateTime> Dates => FixedFlows.Select(e => e.PaymentDate);
        public IEnumerable<Double> Values => FixedFlows.Select(e => e.Amount);
        public required Currency Currency { get; set; } // ignored for the moment
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs =>
            FixedFlows.Select(e => new DeterministicPayoff() {
                Maturity = e.PaymentDate,
                PaymentDate = e.PaymentDate,
                PayoffValue = e.Amount,
                Currency = Currency
            });
        public double Notional { get; set; } = 1.0;
    }
}
