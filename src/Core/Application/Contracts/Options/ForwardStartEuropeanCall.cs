using Domain;

namespace Application {
    public class ForwardStartEuropeanCall : SinglePayoffPathDependentContract, ICall {
        public required DateTime StartDate { get; init; }
        public double Factor { get; init; } = 1.0;
        public required Underlying Underlying { get; init; }
        public required Currency Currency { get; init; }
        public override IPathDependentPayoff Payoff => new MonoUnderlyingPathDependentPayoff() { 
            PayoffMap = d => Notional * Math.Max(0, d[Maturity] - Factor * d[StartDate]),
                ObservationDates = [StartDate, Maturity],
                Underlying = Underlying,
                MonitoringFrequency = MonitoringFrequency.None,
                Currency = Currency,
                Maturity = Maturity,
                PaymentDate = Maturity
            };
        }
}
