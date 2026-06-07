using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}