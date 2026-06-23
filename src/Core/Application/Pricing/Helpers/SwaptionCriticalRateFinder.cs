using Domain;
using MathNet.Numerics.Distributions;

namespace Application {
    /// <summary>
    /// Jamshidian, F. (1989). "An Exact Bond Option Formula." The Journal of Finance, 44(1), 205–209.
    /// </summary>
    public class SwaptionCriticalRateFinder {

        private static Func<double, double> N = x => Normal.CDF(0, 1, x);

        private const int MAX_ITERATIONS = 100;
        private const double MAX_ERROR = 1e-6;

        public static double FindCriticalRate(Swaption swaption, Vasicek model) {
            DateTime swaptionExpiry = swaption.Expiry;
            double inf = 0;
            double sup = 1;
            for (int i = 0; i < MAX_ITERATIONS; i++) {
                double rate = (inf + sup) / 2.0;
                double sum = 0;
                foreach (CashFlow flow in swaption.Swap.FixedFlows) {
                    DateTime date = flow.PaymentDate;
                    sum += flow.Amount * model.DiscountFactor(rate, (date - swaptionExpiry).TotalYears);
                }
                sum += model.DiscountFactor(rate, (swaption.Swap.FixedFlows.Last().PaymentDate - swaptionExpiry).TotalYears);
                // Binary search
                if (sum > 1) {
                    inf = rate;
                } else {
                    sup = rate;
                }
                if (sup - inf < MAX_ERROR) {
                    return rate;
                }
            }
            throw new InvalidOperationException("Failed to converge to a solution within the maximum number of iterations.");
        }

        public static double Price(Swaption swaption, Vasicek model, DateTime valuationDate, double currentRate) {
            double criticalRate = FindCriticalRate(swaption, model);
            DateTime expiryDate = swaption.Expiry;
            List<CashFlow> fixedFlows = swaption.Swap.FixedFlows.ToList();
            List<double> impliedStrikes = fixedFlows.Select(
                flow => model.DiscountFactor(criticalRate, (flow.PaymentDate - expiryDate).TotalYears))
                .ToList();
            double swaptionPrice = 0;
            int last = fixedFlows.Count - 1;
            for (int i = 0; i < fixedFlows.Count; i++) {
                CashFlow flow = fixedFlows[i];
                DateTime date = flow.PaymentDate;
                double sigmaPi = model.B((date - expiryDate).TotalYears) * model._sigma * Math.Sqrt((1 - Math.Exp(-2 * model._kappa * (expiryDate - valuationDate).TotalYears)) / (2 * model._kappa));
                double P_t_Ti = model.DiscountFactor(currentRate, (date - valuationDate).TotalYears);
                double P_t_T0 = model.DiscountFactor(currentRate, (expiryDate - valuationDate).TotalYears);
                double hi = Math.Log(P_t_Ti / (impliedStrikes[i] * P_t_T0)) / sigmaPi + 0.5 * sigmaPi;
                double zeroBondPut = impliedStrikes[i] * P_t_T0 * N(-hi + sigmaPi) - P_t_Ti * N(-hi);
                swaptionPrice += (flow.Amount + (i == last ? 1 : 0)) * zeroBondPut;
            }
            return swaptionPrice;
        }
    }
}
