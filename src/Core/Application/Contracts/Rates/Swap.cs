using Domain;

namespace Application {
    public class Swap : IFlowsContract {
        public IEnumerable<CashFlow> FixedFlows { get; init; }
        public ShortRate FloatingRate { get; init; }

        public Swap(ShortRate floatingRate, double fixedRate, IEnumerable<DateTime> paymentDates) {
            FixedFlows = paymentDates.Select(date => new CashFlow {
                PaymentDate = date,
                Amount = fixedRate,
                Currency = floatingRate.Currency
            });
            FloatingRate = floatingRate;
        }

        public IEnumerable<DateTime> Dates => FixedFlows.Select(e => e.PaymentDate);
        public IEnumerable<Double> Values => FixedFlows.Select(e => e.Amount);
        public required Currency Currency { get; set; }
        public IEnumerable<IPathIndependentPayoff> PathIndependentPayoffs => GetFlows();
        public double Notional { get; set; } = 1.0;

        public IList<IFlow> Flows => (IList<IFlow>) GetFlows();

        public IEnumerable<IPathIndependentPayoff> GetFlows() {
            foreach (CashFlow fixedFlow in FixedFlows) {
                yield return new MonoUnderlyingPathIndependentPayoff() {
                    Payoff = (floating) => floating - fixedFlow.Amount,
                    Underlying = FloatingRate,
                    Maturity = fixedFlow.PaymentDate,
                    PaymentDate = fixedFlow.PaymentDate,
                    Currency = Currency,
                };
            }
        }
    }
}
