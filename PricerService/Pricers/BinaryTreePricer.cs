using Application;
using Domain;

namespace PricerServices.Pricers {
    public class BinaryTreePricer : INonPathDependentPricer {

        private TreeNode? _root;
        private TreeNode? _richardsonExtrapolationRoot;

        private Func<List<DateTime>, List<DateTime>> _intermediateDatesGenerator = (dates) => dates
            .Zip(dates.Skip(1), (start, end) => Enumerable
            .Range(0, 2)
            .Select(i => start.AddDays((end - start).TotalDays * i / 2)))
            .SelectMany(x => x)
            .Append(dates[^1])
            .ToList();

        class TreeNode {
            public DateTime Date { get; set; }
            public double Price { get; set; }
            public TreeNode? Up { get; set; }
            public TreeNode? Down { get; set; }
            private DateTime? NextDate => Up?.Date;
            private double TimeSpan => NextDate.HasValue ? (NextDate.Value - Date).TotalYears : 0;
            // How to improve it to local volatility ? ud != du so no cache possible, see The Volatility Smile and Its Implied Tree by Derman and Kani
            private readonly double _volatility;

            public TreeNode(double price, double volatility, IEnumerable<DateTime> dates, int step = 0, int upMoves = 0, Dictionary<(int step, int upMoves), TreeNode>? cache = null) {
                cache ??= new();
                Price = price;
                Date = dates.First();
                cache[(step, upMoves)] = this;
                IEnumerable<DateTime> remainingDates = dates.Skip(1);
                _volatility = volatility;
                if (remainingDates.Any()) {
                    double timeSpan = (remainingDates.First() - Date).TotalYears;
                    double u = GetUpFactor(volatility, timeSpan);
                    double d = 1 / u;
                    if (!cache.TryGetValue((step + 1, upMoves + 1), out var upNode)) {
                        upNode = new TreeNode(price * u, volatility, remainingDates, step + 1, upMoves + 1, cache);
                    }
                    Up = upNode;
                    if (!cache.TryGetValue((step + 1, upMoves), out var downNode)) {
                        downNode = new TreeNode(price * d, volatility, remainingDates, step + 1, upMoves, cache);
                    }
                    Down = downNode;
                }
            }

            public double GetValue(INonPathDependentPayoff payoff, IDiscounter discounter, int step = 0, int upMoves = 0, Dictionary<TreeNode, double>? cache = null) {
                cache ??= new();
                if (cache.TryGetValue(this, out double cached)) {
                    return cached;
                }
                double value;
                if (Up == null || Down == null) {
                    Underlying underlying = payoff.Dependencies.First();
                    value = payoff.GetPayoffAtMaturity(new Dictionary<Underlying, double> { { underlying, Price } });
                } else {
                    double p = GetUpProbability(_volatility, discounter);
                    value = discounter.GetDiscountFactor(Up.Date, Date) * (p * Up.GetValue(payoff, discounter, step + 1, upMoves + 1, cache) + (1 - p) * Down.GetValue(payoff, discounter, step + 1, upMoves, cache));
                }
                cache[this] = value;
                return value;
            }

            private double GetUpProbability(double volatility, IDiscounter discounter) {
                double u = GetUpFactor(volatility, TimeSpan);
                double d = 1/u;
                return ((1/discounter.GetDiscountFactor(NextDate.Value, Date)) - d) / (u - d);
            }

            private double GetUpFactor(double volatility, double timeSpan) {
                return Math.Exp(volatility * Math.Sqrt(timeSpan));
            }
        }

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization) {
            List<Underlying> underlyings = marketData.GetUnderlyings();
            if (underlyings.Count != 1) {
                throw new ArgumentException("Binary tree pricer only supports single underlying payoffs");
            }
            Underlying underlying = underlyings[0];
            IUnderlyingMarketData underlyingMarketData = marketData.GetUnderlyingMarketData(underlying);
            double spot = underlyingMarketData.GetSpot();
            double volatility = underlyingMarketData.GetVolatility().getVolatility(spot, 0);
            _root = new TreeNode(spot, volatility, timeDiscretization);
            _richardsonExtrapolationRoot = new TreeNode(spot, volatility, _intermediateDatesGenerator(timeDiscretization));
        }

        public PriceWithPrecision Price(INonPathDependentPayoff payoff, IDiscounter discounter, DateTime maturity, DateTime today) {
            if (_root == null || _richardsonExtrapolationRoot == null) {
                throw new InvalidOperationException("Pricer not initialized. Call Initialize() before pricing.");
            }
            double p1 = _root.GetValue(payoff, discounter);
            double p2 = _richardsonExtrapolationRoot.GetValue(payoff, discounter);

            // First-order Richardson extrapolation: eliminates the O(1/n) term
            double extrapolated = 2 * p2 - p1;
            double precision = Math.Abs(p2 - p1); // rough error estimate
            return new PriceWithPrecision() {
                Value = extrapolated,
                Precision = precision,
                Currency = payoff.Currency,
            };
        }
    }
}