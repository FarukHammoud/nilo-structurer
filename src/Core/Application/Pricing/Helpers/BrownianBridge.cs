using Domain;

namespace Application {
    public class BrownianBridge {
        private double _left;
        private double _right;
        private double _spacing;
        private double _volatility;
        private double _min => Math.Min(_left, _right);
        private double _max => Math.Max(_left, _right);
        private static readonly IDayCountConvention _dayCountConvention = new Actual365();
        public BrownianBridge(double left, double right, double spacing, double volatility) {
            _left = left;
            _right = right;
            _spacing = spacing;
            _volatility = volatility;
        }

        /// <summary>
        /// Calculates the cumulative distribution function (CDF) of the Maximum in a Brownian bridge at a given point x.
        /// P(MAX <= x)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double MaxCdf(double x) {
            if (x <= _max) {
                return 0;
            }
            double a = (_max - _min);
            x -= _min;
            return 1 - Math.Exp(-2 * x * (x - a) / (_volatility * _volatility * _spacing));
        }

        /// <summary>
        /// Calculates the cumulative distribution function (CDF) of the Minimum in a Brownian bridge at a given point x.
        /// P(MIN <= x)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double MinCdf(double x) {
            if (x >= _min) {
                return 1;
            }
            double a = (_max - _min);
            x -= _max;
            return Math.Exp(-2 * x * (x + a) / (_volatility * _volatility * _spacing));
        }

        public double ExpectedMax() {
            return _max + (_volatility * Math.Sqrt(Math.PI * _spacing / 8));
        }

        public double ExpectedMin() {
            return _min - (_volatility * Math.Sqrt(Math.PI * _spacing / 8));
        }

        public bool RandomWalkCrossesUpBarrier(double upBarrier, Random random) {
            if (_max >= upBarrier) {
                return true;
            }
            double u = random.NextDouble();
            double pMax = MaxCdf(upBarrier);
            return u > pMax;
        }

        public bool RandomWalkCrossesDownBarrier(double downBarrier, Random random) {
            if (_min <= downBarrier) {
                return true;
            }
            double u = random.NextDouble();
            double pMin = MinCdf(downBarrier);
            return u < pMin;
        }

        public static bool HasCrossed(SimulatedPath path, IList<DateTime> observationDates, double barrierLevel, bool isUp, double volatility, Random random) {
            for (int i = 1; i < observationDates.Count; i++) {
                double left = path.Values[i - 1];
                double right = path.Values[i];
                double spacing = _dayCountConvention.YearFraction(observationDates[i - 1], observationDates[i]);
                BrownianBridge bridge = new BrownianBridge(left, right, spacing, volatility);
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
