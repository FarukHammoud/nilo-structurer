using System.Diagnostics;
using Domain;

namespace Common.Tests {
    public static class StatisticalAssert {

        /// <summary>
        /// Asserts that a value is likely to come from a normal distribution with the specified mean and standard deviation (Z-Test).
        /// </summary>
        public static void IsNormallyDistributed(double value, Estimate estimate, double alpha = 0.001) {
            IsNormallyDistributed(value, estimate.Value, estimate.StandardError, alpha);
        }

        /// <summary>
        /// Asserts that a value is likely to come from a normal distribution with the specified mean and standard deviation (Z-Test).
        /// </summary>
        public static void IsNormallyDistributed(double value, double mean, double stdDev, double alpha = 0.001) {
            // deviation in standard deviations (or sigma units)
            var zScore = (value - mean) / stdDev;
            // probability of observing a value as extreme or more extreme than the observed value, two tailed test
            var pValue = 2 * (1 - MathNet.Numerics.Distributions.Normal.CDF(0, 1, Math.Abs(zScore))); 
            Debug.WriteLine($"Z-Score: {zScore:F3}, P-Value: {pValue:F5}");
            if (pValue <= alpha || double.IsNaN(pValue)) {
                Assert.Fail(
                    $"Statistical Assert Failed: Value {value:F3} is significantly different from the expected normal distribution.\n" +
                    $"P-Value: {pValue:F5} (Threshold α={alpha})\n" +
                    $"Expected Mean: {mean:F3}, Expected StdDev: {stdDev:F3}"
                );
            }
        }

        
    }
}
