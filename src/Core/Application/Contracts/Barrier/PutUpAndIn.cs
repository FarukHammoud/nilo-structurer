using Domain;

namespace Application {
    public class PutUpAndIn : IContract {
        public IEnumerable<IFlow> Flows => [
            new UpAndInPayoff(
                new MonoUnderlyingPathIndependentPayoff() { 
                    Payoff = spot => Math.Max(0, Strike - spot), 
                    Underlying = Underlying, 
                    Currency = Currency,
                    Maturity = Maturity,
                    PaymentDate = Maturity}
                , BarrierLevel, Underlying, StartDate)];
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Strike { get; set; }
        public required double BarrierLevel { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; }
    }
}
