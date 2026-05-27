using MathNet.Numerics.Interpolation;
using MathNet.Numerics.LinearAlgebra;

namespace Domain {
    public class FiniteDifferenceBsPdeSolver {

        private readonly int NS;
        private readonly int NT;

        public FiniteDifferenceBsPdeSolver(int numberOfSteps = 200, int numberOfTimeSteps = 200) { 
            NS = numberOfSteps;
            NT = numberOfTimeSteps;
        }

        public double PriceAmerican(Func<double, double> payoff, double spot, double volatility, double riskFreeInterestRate, double maturity, double dividends) {
            return Price(payoff, spot, volatility, riskFreeInterestRate, maturity, dividends, isAmerican: true);
        }

        public double PriceEuropean(Func<double, double> payoff, double spot, double volatility, double riskFreeInterestRate, double maturity, double dividends) {
            return Price(payoff, spot, volatility, riskFreeInterestRate, maturity, dividends, isAmerican: false);
        }

        private double Price(Func<double, double> payoff, double spot, double volatility, double riskFreeInterestRate, double maturity, double dividends, bool isAmerican = false) {
            // 1. Define grid boundaries
            double Smax = 4.0 * spot;
            double Δt = maturity / NT;
            double ΔS = Smax / NS;
            double r = riskFreeInterestRate;
            double q = dividends;
            double σ = volatility;

            Func<int, double> α = i => 0.5 * Δt * ((r - q) * i - Math.Pow(σ * i, 2));
            Func<int, double> β = i => 1.0 + Δt * (Math.Pow(σ * i, 2) + r);
            Func<int, double> γ = i => -0.5 * Δt * ((r - q) * i + Math.Pow(σ * i, 2));

            // 2. Initialize grid assets
            double[] S = Enumerable.Range(0, NS + 1).Select(i => i * ΔS).ToArray();

            // Populate terminal vector V at t=T using the passed lambda payoff
            Vector<double> V = Vector<double>.Build.Dense(NS + 1, i => payoff(S[i]));

            // 3. Set up the Tridiagonal Matrix A (Implicit Scheme)
            var A = Matrix<double>.Build.Sparse(NS - 1, NS - 1);
            for (int i = 1; i < NS; i++) {
                int row = i - 1;
                if (row > 0) {
                    A[row, row - 1] = α(i);
                }
                A[row, row] = β(i);
                if (row < NS - 2) {
                    A[row, row + 1] = γ(i);
                }
            }

            // 4. Time-stepping backward from known payoff at t=T to unknown t=0
            for (int n = NT - 1; n >= 0; n--) {
                var B = Vector<double>.Build.Dense(NS - 1, i => V[i + 1]);

                // General Boundary Approximation (Linear boundary)
                
                double boundaryMin = 2 * V[1] - V[2];
                double boundaryMax = 2 * V[NS - 1] - V[NS - 2];
                B[0] -= α(1) * boundaryMin;
                B[NS - 2] -= γ(NS - 1) * boundaryMax;

                // Solve system
                Vector<double> V_interior = Vector<double>.Build.Dense(NS - 1);
                A.Solve(B, V_interior); // B and V_interior have NS-1 elements instead of NS+1, does not contain boundaries

                // Reconstruct full grid
                for (int i = 1; i < NS; i++) {
                    V[i] = V_interior[i - 1];
                }

                V[0] = boundaryMin;
                V[NS] = boundaryMax;

                // For American options, apply early exercise condition
                if (isAmerican) {
                    for (int i = 0; i <= NS; i++) {
                        V[i] = Math.Max(V[i], payoff(S[i]));
                    }
                }
            }

            // 5. Interpolate for S0
            var interpolation = LinearSpline.Interpolate(S, V);
            return interpolation.Interpolate(spot);
        }
    }
}