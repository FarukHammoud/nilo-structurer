using Domain;

namespace Application {
    public static class CorrelationMatrixBuilder {
        public static double[,] GetCorrelationMatrix(IMarketData marketData) {
            IList<Underlying> underlyings = marketData.Underlyings;
            int n = underlyings.Count;
            double[,] correlationMatrix = new double[n, n];
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    correlationMatrix[i, j] = marketData.GetCorrelation(underlyings[i], underlyings[j]);
                }
            }
            return correlationMatrix;
        }
    }
}
