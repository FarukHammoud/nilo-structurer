

using Application;
using Domain;

namespace PricingServices.Tests {
    [TestClass]
    public class DupireValidationTests {

        private static double[] LogSpace(double min, double max, int n) {
            double logMin = Math.Log(min);
            double logMax = Math.Log(max);
            double step = (logMax - logMin) / (n - 1);

            var result = new double[n];
            for (int i = 0; i < n; i++)
                result[i] = Math.Exp(logMin + i * step);

            return result;
        }

        [TestMethod]
        public void DupireReturnsSameValuesOnConstantVolatility() {
            double volatility = 0.2;
            IImpliedVolatilityModel constantVolatility = new ConstantVolatilityModel(volatility);
            IDiscounter discounter = new FixedRateDiscounter() { Rate = 0.05 };
            Dupire dupireModel = new Dupire(constantVolatility, discounter);
            double[] spots = LogSpace(1E-6, 1E4, 13);
            double[] daysToMaturity = LogSpace(1, 10.0 * 365.0, 21);
            foreach (double spot in spots) {
                foreach (double days in daysToMaturity) {
                    DateTime maturity2 = DateTime.Today.AddDays(days);
                    double localVol2 = dupireModel.GetLocalVolatility(spot, maturity2, 100, DateTime.Today);
                    Assert.AreEqual(volatility, localVol2, 1e-4,
                        $"Flat surface should reproduce flat local vol at spot={spot:F6}, days={days:F6}");
                }
            }
        }
    }
}
