using MathNet.Numerics.LinearAlgebra;

namespace Domain {
    internal class LongstaffSchwartzAmericanSimulation {

        public double PriceAmerican(Func<double, double> payoff, List<DateTime> callableDates, Realizations realizations, IDiscounter discounter) {
            // 1. Parameters
            int N = realizations.Size;
            int steps = callableDates.Count;
            // 2. Initialize cash flows matrix
            Matrix<double> cashFlows = Matrix<double>.Build.Dense(N, steps);
            // 3. Set terminal cash flows at maturity set to payoff
            cashFlows.SetColumn(steps - 1, realizations.Paths.Select(path => payoff(path.Last)).ToArray());
            for (int i = N - 2; i >= 0; i--) {
                bool[] inTheMoney = realizations.Paths.Select(path => payoff(path[i]) > 0).ToArray();
                // In-the-money paths only
                int[] itmIndices = Enumerable.Range(0, N).Where(j => inTheMoney[j]).ToArray();
                if (itmIndices.Length == 0) continue;
                // x = prices
                Vector<double> x = Vector<double>.Build.DenseOfArray(realizations.Paths.Where((path, j) => inTheMoney[j])
                    .Select(path => path[i]).ToArray());
                // y = discounted next cash flows
                double stepDiscountFactor = discounter.GetDiscountFactor(callableDates[i + 1], callableDates[i]);
                Vector<double> y = Vector<double>.Build.DenseOfArray(realizations.Paths.Where((path, j) => inTheMoney[j])
                    .Select(path => path[i + 1] * stepDiscountFactor).ToArray());
                // Fit regression to estimate continuation value
                Matrix<double> X = Matrix<double>.Build.DenseOfColumnArrays(
                    Enumerable.Repeat(1.0, x.Count).ToArray(),
                    x.ToArray(),
                    x.PointwisePower(2).ToArray()
                );
                Vector<double> beta = X.Solve(y); 
                Vector<double> expectedContinuationPayoff = X.Multiply(beta).Map(payoff);
                bool[] optimum_exercise = x.Map(payoff).Zip(expectedContinuationPayoff, (a, b) => a > b).ToArray();
                Vector<double> continuationValue = X.Multiply(beta);
                // Exercise decision
                for (int k = 0; k < itmIndices.Length; k++) {
                    int j = itmIndices[k];
                    double exerciseValue = payoff(realizations.Paths[j][i]);
                    if (exerciseValue > continuationValue[k]) {
                        // Exercise: set cash flow here, zero out future cash flows
                        cashFlows[j, i] = exerciseValue;
                        for (int t = i + 1; t < steps; t++)
                            cashFlows[j, t] = 0;
                    }
                }
            }
            // Price = average discounted cash flow across all paths
            Vector<double> cashFlow = cashFlows.ColumnSums().Divide(N);
            double price = Enumerable.Range(0, steps)
                .Sum(t => cashFlow[t] * discounter.GetDiscountFactor(callableDates[t], callableDates[0]));

            return price;
        }
    }
}
