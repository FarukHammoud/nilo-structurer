using Domain;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;
namespace Application {
    /// <summary>
    /// Derman-Kani, The volatility smile and its implied tree, 1994, Journal of Financial and Quantitative Analysis, 29(4), 611-650
    /// </summary>
    public class DermanKaniBinaryTreePricer : PayoffPricer, IPricer {
        private static IDayCountConvention _dayCountConvention = new Actual365();

        private double _spot;
        private Underlying _underlying;
        private ILocalVolatilityModel _volatility; // SHOULD BE IImpliedVolatilityModel 
        private IDiscounter _discounter;
        private IList<DateTime> _dates;

        private Matrix<double> _impliedStockPrices;
        private Matrix<double> _transitionProbabilities;
        private Matrix<double> _arrowDebreuPrices;

        private bool _withPrecision;
        private bool _withBarleCakiciCentering; // Barle-Cakici (1998) centers the tree on the forward, and not the spot
        private DermanKaniBinaryTreePricer _richardsonExtrapolation;

        public DermanKaniBinaryTreePricer() {
            _withPrecision = true;
        }

        public DermanKaniBinaryTreePricer(bool withPrecision) {
            _withPrecision = withPrecision;
        }

        // alias
        private Matrix<double> S => _impliedStockPrices;
        private Matrix<double> p => _transitionProbabilities;
        private Matrix<double> λ => _arrowDebreuPrices;
        private int N => _dates.Count;
        private Func<int, double> DF => (n) => _discounter.GetDiscountFactor(_dates[n], _dates[n + 1]);
        private Func<int, double> Forward => (n) => 1 / DF(n);

        private Func<IList<DateTime>, IList<DateTime>> _intermediateDatesGenerator = (dates) => dates
            .Zip(dates.Skip(1), (start, end) => Enumerable
            .Range(0, 2)
            .Select(i => start.AddDays((end - start).TotalDays * i / 2)))
            .SelectMany(x => x)
            .Append(dates[^1])
            .ToList();

        public override void Initialize(IMarketData marketData, IList<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            base.Initialize(marketData, timeDiscretization, pricerConfiguration);
            if (pricerConfiguration is DermanKaniPricerConfiguration configuration) {
                _withBarleCakiciCentering = configuration.WithBarleCakiciCentering;
            }
            IList<Underlying> underlyings = marketData.Underlyings;
            if (underlyings.Count != 1) {
                throw new ArgumentException("Binary tree pricer only supports single underlying payoffs");
            }
            _underlying = underlyings.First();
            IUnderlyingMarketData underlyingMarketData = marketData.GetUnderlyingMarketData(_underlying);
            _spot = underlyingMarketData.GetSpot();
            _volatility = underlyingMarketData.GetVolatility();
            _discounter = marketData.GetDiscounter(_underlying.Currency);
            _dates = timeDiscretization;
            BuildMatrices();
            if (_withPrecision) {
                _richardsonExtrapolation = new DermanKaniBinaryTreePricer(false);
                _richardsonExtrapolation.Initialize(marketData, _intermediateDatesGenerator(timeDiscretization), pricerConfiguration);
            }
        }

        public override PriceEstimate PricePayoff(IPayoff payoff, DateTime today, Currency pricingCurrency) {
            int maturityIndex = _dates.IndexOf(payoff.Maturity);
            if (maturityIndex < 0 || payoff is not IPathIndependentPayoff europeanPayoff) {
                throw new ArgumentException("Payoff maturity does not align with tree dates");
            }

            double price = 0;
            for (int i = 0; i <= maturityIndex; i++) {
                double spot = S[maturityIndex, i];
                price += λ[maturityIndex, i] * europeanPayoff.ComputePayoff(new Dictionary<Underlying, double> { { _underlying, spot } });
            }

            if (_withPrecision) {
                double finerGridPrice = _richardsonExtrapolation.PricePayoff(payoff, today, pricingCurrency).Value;
                double extrapolated = 2 * finerGridPrice - price;
                double precision = Math.Abs(finerGridPrice - price); // rough error estimate
                return new PriceEstimate(
                    value: extrapolated,
                    standardError: precision,
                    currency: payoff.Currency
                );
            }

            return new PriceEstimate(price, pricingCurrency);
        }

        // Paper Formulas
        private double CallPrice(double strike, double timeToMaturity) {
            double volatility = _volatility.GetVolatility(strike, timeToMaturity);
            double riskFreeRate = _discounter.GetForwardRate(_dates[0], _dates[0].AddDays(timeToMaturity * 365));
            BlackScholes bsModel = new BlackScholes(OptionType.Call, _spot, strike, timeToMaturity, riskFreeRate, volatility);
            return bsModel.Premium;
        }

        private double PutPrice(double strike, double timeToMaturity) {
            double volatility = _volatility.GetVolatility(strike, timeToMaturity);
            double riskFreeRate = _discounter.GetForwardRate(_dates[0], _dates[0].AddDays(timeToMaturity * 365));
            BlackScholes bsModel = new BlackScholes(OptionType.Put, _spot, strike, timeToMaturity, riskFreeRate, volatility);
            return bsModel.Premium;
        }

        private void UpdateTransitionProbabilities(int n) {
            for (int i = 0; i <= n; i++) {
                double si = S[n, i];
                double Si = S[n + 1, i];
                double Si1 = S[n + 1, i + 1];
                double Fi = si * Forward(n);
                p[n, i] = (Fi - Si) / (Si1 - Si);
                p[n, i] = Math.Max(0, Math.Min(1, p[n, i]));
            }
        }

        private void UpdateArrowDebreuPrices(int n) {
            for (int i = 0; i <= n + 1; i++) {
                λ[n + 1, i] = 0;
                if (i > 0) {
                    double pi1 = p[n, i - 1];
                    λ[n + 1, i] += DF(n) * pi1 * λ[n, i - 1];
                }
                if (i < n + 1) {
                    double pi = p[n, i];
                    λ[n + 1, i] += DF(n) * (1 - pi) * λ[n, i];
                }
            }
        }

        private double UpperSum(int n, int i) {
            double si = S[n, i];
            double sum = 0;
            for(int j = i + 1; j < N; j++) {
                double sj = S[n, j];
                double Fj = Forward(n) * sj;
                sum += λ[n, j] * (Fj - si);
            }
            return sum;
        }

        private double LowerSum(int n, int i) {
            double si = S[n, i];
            double sum = 0;
            for (int j = 0; j < i; j++) {
                double sj = S[n, j];
                double Fj = Forward(n) * sj;
                sum += λ[n, j] * (si - Fj);
            }
            return sum;
        }

        // S[i][j+1] as a function of S[i][j] 
        private double UpperFormula(int n, int i) {
            double si = S[n, i];
            double Si = S[n + 1, i];
            double λi = λ[n, i];
            double Fi = Forward(n) * si;
            double call = CallPrice(si, _dayCountConvention.YearFraction(_dates[0], _dates[n + 1]));
            double sum = UpperSum(n, i);
            return (Si * (Forward(n) * call - sum) - λi * si * (Fi - Si))
                / (Forward(n) * call - sum - λi * (Fi - Si));
        }

        // S[n + 1, i] = LowerFormula(n, i);
        private double LowerFormula(int n, int i) {
            double si = S[n, i];
            double Si_1 = S[n + 1, i + 1];
            double λi = λ[n, i];
            double Fi = Forward(n) * si;
            double put = PutPrice(si, _dayCountConvention.YearFraction(_dates[0], _dates[n + 1]));
            double sum = LowerSum(n, i);
            return (Si_1 * (Forward(n) * put - sum) + λi * si * (Fi - Si_1))
                / (Forward(n) * put - sum + λi * (Fi - Si_1));
        }

        private double OddUpper(int n, int i) {
            double si = S[n, i];
            double λi = λ[n, i];
            double Fi = Forward(n) * si;
            double S_ = si; 
            double call = CallPrice(S_, _dayCountConvention.YearFraction(_dates[0], _dates[n + 1]));
            double sum = UpperSum(n, i);
            double oddUpper = (S_ * (Forward(n) * call + λi * S_ - sum))
                / (λi * Fi - Forward(n) * call + sum);
            if (oddUpper < 0) {
                oddUpper *= -1;
            }
            if (_withBarleCakiciCentering) {
                return Forward(n) * oddUpper;
            }
            return oddUpper;
        }

        private void BuildMatrices() {
            _impliedStockPrices = Matrix<double>.Build.Dense(N, N);
            _arrowDebreuPrices = Matrix<double>.Build.Dense(N, N);
            _transitionProbabilities = Matrix<double>.Build.Dense(N, N);

            S[0, 0] = _spot;
            λ[0, 0] = 1;
            for (int n = 0; n < N-1; n++) {
                // from known n, we set n + 1
                if (n.IsEven()) {
                    int up = n/2 + 1;
                    int down = n/2;
                    double S_ = S[n, down];
                    S[n + 1, down + 1] = OddUpper(n, down);
                    if (_withBarleCakiciCentering) {
                        S[n + 1, down] = S_ * S_ * Forward(n) * Forward(n) / S[n + 1, up]; 
                    } else {
                        S[n + 1, down] = S_ * S_ / S[n + 1, up];
                    }
                    
                    // upper nodes
                    for (int i = up; i <= n; i++) {
                        S[n + 1, i + 1] = UpperFormula(n, i);
                        EnforceNoArbitrage(n, i + 1, true);
                    }
                    // lower nodes
                    for (int i = down - 1; i >= 0; i--) {
                        S[n + 1, i] = LowerFormula(n, i);
                        EnforceNoArbitrage(n, i, false);
                    }
                } else if (n.IsOdd()){
                    int central = (n + 1) / 2;
                    int prevDown = (n - 1) / 2;  
                    int prevUp   = (n + 1) / 2;  
                    double geometricCenter = Math.Sqrt(S[n, prevDown] * S[n, prevUp]);
                    if (_withBarleCakiciCentering) {
                        S[n + 1, central] = geometricCenter * Forward(n); 
                    } else {
                        S[n + 1, central] = _spot;
                    }
                    // upper nodes
                    for (int i = central; i <= n; i++) {
                        S[n + 1, i + 1] = UpperFormula(n, i);
                        EnforceNoArbitrage(n, i + 1, true);
                    }
                    // lower nodes
                    for (int i = central - 1; i >= 0; i--) {
                        S[n + 1, i] = LowerFormula(n, i);
                        EnforceNoArbitrage(n, i, false);
                    }
                }
                UpdateTransitionProbabilities(n);
                UpdateArrowDebreuPrices(n);
            }
            Debug.WriteLine($"Implied Stock Prices (S)");
            Debug.WriteLine(S.ToMatrixString(50, 50));
            Debug.WriteLine($"Transition Probabilities (p)");
            Debug.WriteLine(p.ToMatrixString(50, 50));
            Debug.WriteLine($"Arrow-Debreu Prices (λ)");
            Debug.WriteLine(λ.ToMatrixString(50, 50));
        }

        // Enforces Fi < Si < Fi+1 for node just computed at S[n+1, i],
        private void EnforceNoArbitrage(int n, int i, bool upward) {
            double value = S[n + 1, i];
            if (i > 0 && value < Forward(n) * S[n, i - 1] ||
                i < n && value > Forward(n) * S[n, i]) {
                if (upward) {
                    // Building UPWARDS: rely on S[n+1, i-1] (already known)
                    // Use parent ratio S[n, i-1] / S[n, i-2] (with fallback if i < 2)
                    double parentLogRatio = (i >= 2)
                        ? (S[n, i - 1] / S[n, i - 2])
                        : (S[n, i] / S[n, i - 1]);

                    S[n + 1, i] = S[n + 1, i - 1] * parentLogRatio;
                } else {
                    // Building DOWNWARDS: rely on S[n+1, i+1] (already known)
                    // Use parent ratio S[n, i+1] / S[n, i] (with fallback if i >= n)
                    double parentLogRatio = (i < n)
                        ? (S[n, i + 1] / S[n, i])
                        : (S[n, i] / S[n, i - 1]);

                    S[n + 1, i] = S[n + 1, i + 1] / parentLogRatio;
                }
                Debug.WriteLine($"Enforcing no arbitrage: S[{n + 1}, {i}] adjusted from {value} to {S[n + 1, i]}");
            }
        }
    }
}