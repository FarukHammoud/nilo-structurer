using MathNet.Numerics.LinearAlgebra;

namespace Domain {
    // Longstaff-Schwartz: Valuying American Options by Simulation, 2001 
    public class LongstaffSchwartzAmericanSimulation {
        private IRegressionBasis _regressionBasis;
        private const int REGRESSION_DEGREE = 3;
        public LongstaffSchwartzAmericanSimulation(IRegressionBasis? regressionBasis = null) {
            _regressionBasis = regressionBasis ?? new LaguerreRegressionBasis(REGRESSION_DEGREE);
        }

        public ValueWithPrecision PriceAmerican(Func<double, double> payoff, DateTime valuationDate, List<DateTime> callableDates, Realizations realizations, IDiscounter discounter) {
            IReadOnlyList<SimulatedPath> paths = realizations.Paths;
            int N = realizations.Size;
            int steps = callableDates.Count;
            Matrix<double> cashFlows = Matrix<double>.Build.Dense(N, steps);
            // Set terminal cash flows at maturity set to payoff
            cashFlows.SetColumn(steps - 1, paths.Select(path => payoff(path.Last)).ToArray());
            for (int step = steps - 2; step >= 0; step--) {
                bool[] ITM = paths.Select(path => payoff(path[step]) > 0).ToArray();
                if (!ITM.Any(x => x)) continue;

                int[] itmIndices = Enumerable.Range(0, N).Where(j => ITM[j]).ToArray();
                double[] continuationValues = EstimateContinuationValues(paths, cashFlows, step, itmIndices, callableDates, discounter);
                double[] exerciseValues = itmIndices.Select(j => payoff(paths[j][step])).ToArray();

                // Exercise decision
                for (int k = 0; k < itmIndices.Length; k++) {
                    if (exerciseValues[k] > continuationValues[k]) {
                        int j = itmIndices[k];
                        cashFlows.ClearRow(j);
                        cashFlows[j, step] = exerciseValues[k];
                    }
                }
            }
            // Price = average discounted cash flow across all paths
            IEnumerable<double> pathPrices = Enumerable.Range(0, N)
                .Select(j => GetDiscountedCashFlow(cashFlows, j, 0, callableDates, discounter, valuationDate));
            double price = pathPrices.Average();
            return new ValueWithPrecision(pathPrices);
        }

        private double GetDiscountedCashFlow(Matrix<double> cashFlows, int j, int fromStep, List<DateTime> callableDates, IDiscounter discounter, DateTime valuationDate) {
            int steps = cashFlows.ColumnCount;
            for (int t = fromStep; t < steps; t++) {
                if (cashFlows[j, t] != 0) {
                    return cashFlows[j, t] * discounter.GetDiscountFactor(callableDates[t], valuationDate);
                }
            }
            return 0.0;
        }

        private double[] EstimateContinuationValues(IReadOnlyList<SimulatedPath> paths, Matrix<double> cashFlows, int step, int[] itmIndices, List<DateTime> callableDates, IDiscounter discounter) {
            // x = (normalized) prices, y = discounted next cash flows
            double spot = paths.Average(path => path[0]);
            Vector<double> x = Vector<double>.Build.DenseOfArray(itmIndices
                .Select(j => paths[j][step] / spot).ToArray());
            Vector<double> y = Vector<double>.Build.DenseOfArray(itmIndices
                .Select(j => GetDiscountedCashFlow(cashFlows, j, step + 1, callableDates, discounter, callableDates[step])).ToArray());
            if (itmIndices.Length < REGRESSION_DEGREE) {
                return y.ToArray();
            }
            // Fit regression to estimate continuation value
            Matrix<double> X = _regressionBasis.Build(x);
            Vector<double> beta = X.Solve(y);
            Vector<double> continuationValues = X.Multiply(beta);
            return continuationValues.ToArray();
        }
    }
}
