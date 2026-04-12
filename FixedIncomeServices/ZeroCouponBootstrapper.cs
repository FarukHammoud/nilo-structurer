using Application;

namespace FixedIncomeServices {
    public class ZeroCouponBootstrapper {

        public static Curve ZeroCouponCurve(Curve swapCurve) {
            Curve discountCurve = GetDiscountCurve(swapCurve);
            Curve zeroCouponCurve = new Curve();
            OrderedDictionary<DateTime, double> discountNodes = discountCurve.GetNodes();
            foreach (var node in discountNodes) {
                DateTime date = node.Key;
                double years = (node.Key - DateTime.Now).TotalDays / 365.0;
                double discountFactor = node.Value;
                double zeroRate = -Math.Pow(discountFactor, -1 / years) - 1; // annual compounding
                zeroCouponCurve.setNode(date, zeroRate);
            }
            return zeroCouponCurve;
        }

        public static Curve GetDiscountCurve(Curve swapCurve) {
            Curve discountCurve = new Curve();
            discountCurve.setNode(DateTime.Now, 1);
            if (!swapCurve.FirstDate.HasValue || !swapCurve.LastDate.HasValue) {
                return discountCurve;
            }
            for (int i = 0; i < (swapCurve.LastDate.Value - swapCurve.FirstDate.Value).TotalDays / 365.0; i++) {
                DateTime date = swapCurve.FirstDate.Value.AddDays(365 * i);
                double SwapRate = swapCurve.GetValue(date);
                double sum = 0;
                for (int j = 1; j <= i; j++) {
                    DateTime date_j = swapCurve.FirstDate.Value.AddDays(365 * j);
                    sum += discountCurve.GetValue(date_j);
                }
                double discountFactor = (1 - SwapRate * sum) / (1 + SwapRate);
                discountCurve.setNode(date, discountFactor);
            }
            return discountCurve;
        }

        public static double GetForwardDiscountFactor(DateTime i, DateTime j, Curve zeroCouponCurve) {
            return zeroCouponCurve.GetValue(j) / zeroCouponCurve.GetValue(i);
        }
    }
}
