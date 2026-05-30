using MathNet.Numerics.LinearAlgebra;

namespace Domain {
    public static class MatrixExtensions {
        /// <summary>
        /// Solves Ax = b using the Thomas algorithm (tridiagonal matrix algorithm).
        /// Requires the matrix to be strictly diagonally dominant or symmetric positive definite.
        /// O(n) time and space
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the matrix is not square or not tridiagonal.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a zero pivot is encountered.</exception>
        public static void ThomasSolve<T>(this Matrix<T> matrix, Vector<T> input, Vector<T> result)
            where T : struct, IEquatable<T>, IFormattable {
            // Cast to double — Thomas is only meaningful for floating point
            var A = matrix.Clone() as Matrix<double>
                ?? throw new ArgumentException("ThomasSolve only supports Matrix<double>.", nameof(matrix));
            var b = input.Clone() as Vector<double>
                ?? throw new ArgumentException("ThomasSolve only supports Vector<double>.", nameof(input));
            var r = result as Vector<double>
                ?? throw new ArgumentException("ThomasSolve only supports Vector<double>.", nameof(result));

            ValidateConditions(A, b, r);
            int n = A.RowCount;

            // eliminate lower diagonal 
            // ignore the lower diagonal item since its never used
            for (int i = 1; i < n; i++) {
                // row_i = row_i - (a_i / b_{i-1}) * row_{i-1}
                if (Math.Abs(A[i - 1, i - 1]) < 1e-14) {
                    throw new InvalidOperationException($"Zero pivot at row {i - 1}. Matrix may not be diagonally dominant.");
                }
                double factor = A[i, i-1] / A[i - 1, i - 1];
                for (int j = i; j <= i + 1 && j < n; j++) {
                    A[i, j] -= factor * A[i - 1, j];
                }
                b[i] -= factor * b[i - 1];
            }

            // eliminate upper diagonal 
            // ignore the upper diagonal item since its never used
            for (int i = n-2; i >= 0; i--) {
                // row_i = row_i - (c_{i} / b_{i+1}) * row_{i+1}
                double factor = A[i, i + 1] / A[i + 1, i + 1];
                b[i] -= factor * b[i + 1];
            }

            // get results from the diagonal
            for (int i = 0; i < n; i++) {
                r[i] = b[i] / A[i, i];
            }
        }

        private static void ValidateConditions(Matrix<double> A, Vector<double> b, Vector<double> r) {
            int n = A.RowCount;

            if (A.ColumnCount != n) {
                throw new ArgumentException("Matrix must be square.", nameof(A));
            }
            if (b.Count != n) {
                throw new ArgumentException("Input length must match matrix dimension.", nameof(b));
            }
            if (r.Count != n) {
                throw new ArgumentException("Result length must match matrix dimension.", nameof(r));
            }
            ValidateTridiagonal(A, n);
        }

        private static void ValidateTridiagonal(Matrix<double> A, int n) {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) {
                    if (Math.Abs(i - j) > 1 && A[i, j] != 0.0) {
                        throw new ArgumentException(
                            $"Matrix is not tridiagonal: non-zero value {A[i, j]} at ({i},{j}).",
                            nameof(A));
                    }
                }
        }
    }
}
