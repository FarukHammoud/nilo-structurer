using Domain;

namespace Application {
    public class CallUpAndIn : IPathDependentContract, IKnockInBarrierContract {
        public IEnumerable<Tuple<DateTime, IPathDependentPayoff>> Payoffs => [];
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Strike { get; set; }
        public required double BarrierLevel { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; }

        public IReadOnlyList<IKnockInBarrier> KnockInBarriers => [new SingleUnderlyingUpAndInBarrier() { 
            ActivatedPayoff = new MonoUnderlyingPathIndependentPayoff(
                payoffMap: S => Math.Max(0, S - Strike),
                currency: Currency,
                underlying: Underlying),
            BarrierLevel = BarrierLevel,
            MonitoringFrequency = MonitoringFrequency.Continuous,
            Underlying = Underlying,
            ObservationDates = [Maturity]
        }];
    }
}
