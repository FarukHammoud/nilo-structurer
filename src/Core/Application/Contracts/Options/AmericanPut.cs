using Domain;

namespace Application {
    public class AmericanPut : IPathDependentContract, IPut {
        public required Underlying Underlying { get; set; }
        public required double Strike { get; set; }
        public required Currency Currency { get; set; }
        public required DateTime Maturity { get; set; }

        public IEnumerable<IPathDependentPayoff> Payoffs => [
            new MonoUnderlyingPathDependentPayoff() {
                PayoffMap = prices => Notional * Math.Max(0, Strike - prices[Maturity]),
                Underlying = Underlying,
                Currency = Currency,
                PaymentDate = Maturity,
                MonitoringFrequency = MonitoringFrequency.Continuous,
                ObservationDates = Enumerable.Range(0, (Maturity - DateTime.Now).Days + 1).Select(i => DateTime.Now.AddDays(i)).ToArray()
            }];

        public double Notional { get; set; } = 1;
    }
}
