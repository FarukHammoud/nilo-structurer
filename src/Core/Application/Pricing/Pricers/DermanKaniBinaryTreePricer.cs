using Domain;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;
namespace Application {
    /// <summary>
    /// Derman-Kani, The volatility smile and its implied tree, 1994, Journal of Financial and Quantitative Analysis, 29(4), 611-650
    /// </summary>
    public class DermanKaniBinaryTreePricer : PayoffPricer, IPricer {

        private double _spot;
        private Underlying _underlying;
        private ILocalVolatilityModel _volatility;
        private IDiscounter _discounter;
        private IList<DateTime> _dates;

        private Matrix<double> _impliedStockPrices;
        private Matrix<double> _transitionProbabilities;
        private Matrix<double> _arrowDebreuPrices;

        // alias
        private Matrix<double> S => _impliedStockPrices;
        private Matrix<double> p => _transitionProbabilities;
        private Matrix<double> λ => _arrowDebreuPrices;
        private int N => _dates.Count;
        private Func<int, double> DF => (n) => _discounter.GetDiscountFactor(_dates[n + 1], _dates[n]);
        private Func<int, double> Forward => (n) => 1 / DF(n);

        // Paper Formulas
        private double CallPrice(double strike, double timeToMaturity) {
            double volatility = _volatility.GetVolatility(strike, timeToMaturity);
            double riskFreeRate = _discounter.GetForwardRate(DateTime.Today, DateTime.Today.AddDays(timeToMaturity * 365));
            BlackScholes bsModel = new BlackScholes(OptionType.Call, _spot, strike, timeToMaturity, riskFreeRate, volatility);
            return bsModel.Premium;
        }

        private double PutPrice(double strike, double timeToMaturity) {
            double volatility = _volatility.GetVolatility(strike, timeToMaturity);
            double riskFreeRate = _discounter.GetForwardRate(DateTime.Today, DateTime.Today.AddDays(timeToMaturity * 365));
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
            double call = CallPrice(si, (_dates[n + 1] - DateTime.Today).TotalYears);
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
            double put = PutPrice(si, (_dates[n+1] - DateTime.Today).TotalYears);
            double sum = LowerSum(n, i);
            return (Si_1 * (Forward(n) * put - sum) + λi * si * (Fi - Si_1))
                / (Forward(n) * put - sum + λi * (Fi - Si_1));
        }

        private double OddUpper(int n, int i) {
            double si = S[n, i];
            double λi = λ[n, i];
            double Fi = Forward(n) * si;
            double S_ = si; 
            double call = CallPrice(S_, (_dates[n + 1] - DateTime.Today).TotalYears);
            double sum = UpperSum(n, i);
            return S_ * (Forward(n) * call + λi * S_ - sum)
                / (λi * Fi - Forward(n) * call + sum);
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
                    S[n + 1, down] = S_ * S_ / S[n + 1, up];
                    // upper nodes
                    for (int i = up; i <= n; i++) {
                        S[n + 1, i + 1] = UpperFormula(n, i);
                        EnforceNoArbitrage(n, i + 1);
                    }
                    // lower nodes
                    for (int i = down - 1; i >= 0; i--) {
                        S[n + 1, i] = LowerFormula(n, i);
                        EnforceNoArbitrage(n, i);
                    }
                } else if (n.IsOdd()){
                    int central = (n + 1)/2;
                    S[n + 1, central] = _spot;
                    // upper nodes
                    for (int i = central; i <= n; i++) {
                        S[n + 1, i + 1] = UpperFormula(n, i);
                        EnforceNoArbitrage(n, i + 1);
                    }
                    // lower nodes
                    for (int i = central - 1; i >= 0; i--) {
                        S[n + 1, i] = LowerFormula(n, i);
                        EnforceNoArbitrage(n, i);
                    }
                }
                UpdateTransitionProbabilities(n);
                UpdateArrowDebreuPrices(n);
            }
            Debug.WriteLine($"Implied Stock Prices (S)");
            Debug.WriteLine(S);
            Debug.WriteLine($"Transition Probabilities (p)");
            Debug.WriteLine(p);
            Debug.WriteLine($"Arrow-Debreu Prices (λ)");
            Debug.WriteLine(λ);
        }

        public override void Initialize(IMarketData marketData, IList<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            base.Initialize(marketData, timeDiscretization, pricerConfiguration);
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

            return new PriceEstimate(price, pricingCurrency);
        }

        // Enforces Fi < Si < Fi+1 for node just computed at S[n+1, i],
        private void EnforceNoArbitrage(int n, int i) {
            double value = S[n + 1, i];
            if (i > 0) {
                double lowerForward = Forward(n) * S[n, i - 1];
                if (value < lowerForward) {
                    S[n + 1, i] = lowerForward + 0.001;
                }
            }
            if (i < n) {
                double upperForward = Forward(n) * S[n, i];
                if (value > upperForward) {
                    S[n + 1, i] = upperForward - 0.001;
                }
            }
        }
    }
}