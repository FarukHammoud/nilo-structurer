using Domain;

namespace Application {
    public class CorridorVarianceSwap : SinglePayoffPathDependentContract {
        public required Underlying Underlying { get; set; }
        public required double VarianceStrike { get; set; }
        public required DateTime StartDate { get; set; }
        public required Currency Currency { get; set; }
        public double UpperBound { get; set; } = double.MaxValue;
        public double LowerBound { get; set; } = 0.0;
        public override IPathDependentPayoff Payoff => 
            new MonoUnderlyingPathDependentPayoff() {
                PayoffMap = GetPayoff,
                ObservationDates = FixingDates,
                Underlying = Underlying,
                MonitoringFrequency = MonitoringFrequency.Daily,
                Currency = Currency,
            };
        
        private List<DateTime> FixingDates => Enumerable.Range(0, (int)(Maturity - StartDate).TotalDays).Select(i => StartDate.AddDays(i)).ToList();
        private double GetPayoff(Dictionary<DateTime, double> prices) {
            return Notional * (GetVariance(prices) - VarianceStrike);
        }

        private double GetVariance(Dictionary<DateTime, double> prices) {
            List<double> priceList = FixingDates.Select(date => prices[date]).ToList();
            int N = 0;
            double sum = 0;
            for (int i = 1; i < priceList.Count; i++) {
                if (priceList[i - 1] < LowerBound || priceList[i - 1] > UpperBound) {
                    continue;
                }
                sum += Math.Pow(Math.Log(priceList[i] / priceList[i - 1]), 2);
                N++;
            }
            return (252 / N) * sum;
        }
    }
}
