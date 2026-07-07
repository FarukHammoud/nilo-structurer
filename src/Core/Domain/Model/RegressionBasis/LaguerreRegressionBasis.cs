using MathNet.Numerics.LinearAlgebra;

namespace Domain {
    // Laguerre polynomials 
    // e^{-x/2} * {1, 1-x, 1 - 2x + x²/2}
    public class LaguerreRegressionBasis : IRegressionBasis {
        private int _degree;

        public LaguerreRegressionBasis(int degree) {
            _degree = degree;
        }
        public Matrix<double> Build(Vector<double> x) {
            int n = x.Count;
            Matrix<double> X = Matrix<double>.Build.Dense(n, _degree + 1);
            for (int i = 0; i < n; i++) { 
                double s = x[i];
                double w = Math.Exp(-s / 2.0);
                X[i, 0] = w * 1.0;
                if (_degree >= 1) { 
                    X[i, 1] = w * (1.0 - s);
                }
                if (_degree >= 2) {
                    X[i, 2] = w * (1.0 - 2.0 * s + 0.5 * s * s);
                }
                if (_degree >= 3) {
                    X[i, 3] = w * (1.0 - 3.0 * s + 1.5 * s * s - s * s * s / 6.0);
                }
                for (int j = 4; j <= _degree; j++) {
                    // Recurrence: L_n(x) = ((2n-1-x)*L_{n-1}(x) - (n-1)*L_{n-2}(x)) / n
                    double L_prev2 = X[i, j - 2] / w;
                    double L_prev1 = X[i, j - 1] / w;
                    double L_curr = ((2.0 * j - 1.0 - s) * L_prev1 - (j - 1.0) * L_prev2) / j;
                    X[i, j] = w * L_curr;
                }
            }
            return X;
        }

        public Matrix<double> Build(List<Vector<double>> xs) {
            throw new NotImplementedException();
        }
    }
}
