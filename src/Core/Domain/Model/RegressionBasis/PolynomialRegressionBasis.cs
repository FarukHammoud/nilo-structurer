using MathNet.Numerics.LinearAlgebra;

namespace Domain {
    public class PolynomialRegressionBasis : IRegressionBasis {
        private int _degree;

        public PolynomialRegressionBasis(int degree) {
            _degree = degree;
        }

        public Matrix<double> Build(Vector<double> x) {
            int n = x.Count;
            Matrix<double> X = Matrix<double>.Build.Dense(n, _degree + 1);
            for (int i = 0; i < n; i++) {
                for (int j = 0; j <= _degree; j++) {
                    X[i, j] = Math.Pow(x[i], j);
                }
            }
            return X;
        }

        public Matrix<double> Build(List<Vector<double>> xs) {
            int variables = xs.Count;
            int vectorLength = xs[0].Count;
            Matrix<double> X = Matrix<double>.Build.Dense(vectorLength, _degree + 1);

            for (int i = 0; i < vectorLength; i++) {
                X[i, 0] = 1.0;
                for (int degree = 1; degree <= _degree; degree++) {
                    for (int variable = 0; variable < variables; variable++) {
                        X[i, variable * _degree + degree] = Math.Pow(xs[variable][i], degree);
                    }
                }
            }
            return X;
        }
    }
}