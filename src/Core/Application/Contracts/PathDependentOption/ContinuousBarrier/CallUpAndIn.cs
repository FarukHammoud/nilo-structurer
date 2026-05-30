using Domain;

namespace Application {
    public class CallUpAndIn : IPathDependentContract, IKnockInBarrierContract {
        public IEnumerable<IPathDependentPayoff> Payoffs => [];
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Strike { get; set; }
        public required double BarrierLevel { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; }
        public IEnumerable<DateTime> Dates => KnockInBarriers[0].ObservationDates;

        public IReadOnlyList<IKnockInBarrier> KnockInBarriers => [new SingleUnderlyingUpAndInBarrier() { 
            ActivatedPayoff = new MonoUnderlyingPathIndependentPayoff() {
                Payoff = spot => Math.Max(0, spot - Strike),
                Currency = Currency,
                Underlying = Underlying,
                Maturity = Maturity,
                PaymentDate = Maturity
            },
            BarrierLevel = BarrierLevel,
            MonitoringFrequency = MonitoringFrequency.Continuous,
            Underlying = Underlying,
            ObservationDates = [Maturity]
        }];
    }
}
