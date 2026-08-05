using Domain;

namespace Application {
    public class Swap : IContract {
        public required IDayCountConvention DayCounter { get; init; }
        public required double FixedRate { get; init; }
        public required ShortRate FloatingRate { get; init; }
        public required IEnumerable<DateTime> Dates { get; init; }
        public required Currency Currency { get; set; }
        public double Notional { get; set; } = 1.0;

        public IEnumerable<CashFlow> FixedFlows => Dates.Select(date => new CashFlow {
            PaymentDate = date,
            Amount = Notional * FixedRate,
            Currency = FloatingRate.Currency
        });

        public IEnumerable<IFlow> Flows => GetFlows();

        // TODO: Should in fact be the forward rate Ti -> Ti+1
        // That is the Log(DiscountFactor(Ti, Ti+1)) / (Ti+1 - Ti)

        private double GetForwardRate(DateTime start, DateTime end, Dictionary<DateTime, double> shortRatePath) {
            double kappa = 0.1;
            double theta = 0.035;
            double sigma = 0.01;
            double dt = DayCounter.YearFraction(start, end);
            Vasicek model = new Vasicek(kappa, theta, sigma);
            double P_T0_T1 = model.DiscountFactor(shortRatePath[start], dt);
            return (1 / P_T0_T1 - 1) / dt;
        }

        public IEnumerable<IPathDependentPayoff> GetFlows() {
            foreach (CashFlow fixedFlow in FixedFlows) {
                yield return new MonoUnderlyingPathDependentPayoff() {
                    PayoffMap = (prices) => Notional * (GetForwardRate(fixedFlow.PaymentDate.AddYears(-1), fixedFlow.PaymentDate, prices) - FixedRate),
                    Underlying = FloatingRate,
                    Maturity = fixedFlow.PaymentDate,
                    PaymentDate = fixedFlow.PaymentDate,
                    Currency = Currency,
                    MonitoringFrequency = MonitoringFrequency.Daily,
                    ObservationDates = [fixedFlow.PaymentDate]
                };
            }
        }
    }
}
