using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Domain;

namespace Application {
    public class ControlVariateReducer : IVarianceReducer {
        private readonly List<List<double>> _controlVariates;
        private readonly List<double> _expectations;
        private readonly List<double> _coefficients;

        public ControlVariateReducer(List<List<double>> controlVariates, List<double> expectations, List<double> payoffs) {
            _controlVariates = controlVariates;
            _expectations = expectations;
            _coefficients = ComputeCoefficients(controlVariates, payoffs);
        }

        public List<double> Adjust(IEnumerable<double> discountedPayoffs) {
            return discountedPayoffs.Select(
                (discountedPayoff, ω) => discountedPayoff + ComputeAdjustment(ω))
                .ToList();
        }

        private double ComputeAdjustment(int pathIndex)
            => Enumerable.Range(0, _controlVariates.Count)
                .Sum(i => _coefficients[i] * (_controlVariates[i][pathIndex] - _expectations[i]));

        private static List<double> ComputeCoefficients(List<List<double>> controlVariates, List<double> payoffs) {
            int n = controlVariates.Count;
            if (n == 0) return [];

            var covariance = Matrix<double>.Build.Dense(n, n, (i, j) =>
                ArrayStatistics.Covariance(controlVariates[i].ToArray(), controlVariates[j].ToArray()));

            var covarianceWithPayoff = Vector<double>.Build.Dense(n, i =>
                -ArrayStatistics.Covariance(controlVariates[i].ToArray(), payoffs.ToArray()));

            return covariance.Solve(covarianceWithPayoff).ToList();
        }
    }
}
