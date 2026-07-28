using Domain;

namespace Application {
    public class CallDownAndOut : IContract {
        public IEnumerable<IFlow> Flows => [ 
            new DownAndOutPayoff(
                new MonoUnderlyingPathIndependentPayoff() {
                    Payoff = spot => Math.Max(0, spot - Strike),
                    Underlying = Underlying,
                    Currency = Currency,
                    PaymentDate = Maturity,
                    Maturity = Maturity
                }, BarrierLevel, Underlying, StartDate)];
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Strike { get; set; }
        public required double BarrierLevel { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; }
    }
}
