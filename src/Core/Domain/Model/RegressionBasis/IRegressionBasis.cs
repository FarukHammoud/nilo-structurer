using MathNet.Numerics.LinearAlgebra;

namespace Domain {
    public interface IRegressionBasis {
        Matrix<double> Build(Vector<double> x);
    }
}
