using Domain;

namespace Application {
    public class PutUpAndIn : IPathDependentContract {
        public IEnumerable<IPathDependentPayoff> Payoffs => [
            new UpAndInPayoff(
                new MonoUnderlyingPathDependentPayoff() { 
                    PayoffMap = d => Math.Max(0, Strike - d.Values.Last()), 
                    ObservationDates =[Maturity], 
                    Underlying = Underlying, 
                    MonitoringFrequency = MonitoringFrequency.Continuous,
                    Currency = Currency,
                    PaymentDate = Maturity}
                , BarrierLevel, Underlying)];
        public required Underlying Underlying { get; set; }
        public required Currency Currency { get; set; }
        public required double Strike { get; set; }
        public required double BarrierLevel { get; set; }
        public required DateTime Maturity { get; set; }
        public double Notional { get; set; }
    }
}
