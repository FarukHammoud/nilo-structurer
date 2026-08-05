using Domain;

namespace Application {
    public class GeometricBrownianBridge {
        private BrownianBridge _bridge;
        private static readonly IDayCountConvention _dayCountConvention = new Actual365();
        public GeometricBrownianBridge(double left, double right, double spacing, double volatility) {
            double logLeft = Math.Log(left);
            double logRight = Math.Log(right);
            _bridge = new BrownianBridge(logLeft, logRight, spacing, volatility);
        }

        /// <summary>
        /// Calculates the cumulative distribution function (CDF) of the Maximum in a Brownian bridge at a given point x.
        /// P(MAX <= x)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double MaxCdf(double x) {
            return _bridge.MaxCdf(Math.Log(x));
        }

        /// <summary>
        /// Calculates the cumulative distribution function (CDF) of the Minimum in a Brownian bridge at a given point x.
        /// P(MIN <= x)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double MinCdf(double x) {
            return _bridge.MinCdf(Math.Log(x));
        }

        public double ExpectedMax() {
            return Math.Exp(_bridge.ExpectedMax());
        }

        public double ExpectedMin() {
            return Math.Exp(_bridge.ExpectedMin());
        }

        public bool RandomWalkCrossesUpBarrier(double upBarrier, Random random) {
            double logUpBarrier = Math.Log(upBarrier);
            return _bridge.RandomWalkCrossesUpBarrier(logUpBarrier, random);
        }

        public bool RandomWalkCrossesDownBarrier(double downBarrier, Random random) {
            double logDownBarrier = Math.Log(downBarrier);
            return _bridge.RandomWalkCrossesDownBarrier(logDownBarrier, random);
        }

        public static bool HasCrossed(SimulatedPath path, IList<DateTime> observationDates, double barrierLevel, bool isUp, double volatility, Random random) {
            for (int i = 1; i < observationDates.Count; i++) {
                double left = path.Values[i - 1];
                double right = path.Values[i];
                double spacing = _dayCountConvention.YearFraction(observationDates[i - 1], observationDates[i]);
                GeometricBrownianBridge bridge = new GeometricBrownianBridge(left, right, spacing, volatility);
                if (isUp) {
                    if (bridge.RandomWalkCrossesUpBarrier(barrierLevel, random)) {
                        return true;
                    }
                } else {
                    if (bridge.RandomWalkCrossesDownBarrier(barrierLevel, random)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
