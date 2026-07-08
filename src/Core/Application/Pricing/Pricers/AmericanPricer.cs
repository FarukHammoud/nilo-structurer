using Domain;
using MathNet.Numerics.LinearAlgebra;

namespace Application {
    public class AmericanPricer : IFlowsPricer {

        private DiffusionConfiguration? _diffusionConfiguration;
        private Diffusion? _diffusion;
        private const int REGRESSION_DEGREE = 3;

        public void Initialize(IMarketData marketData, List<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            if (pricerConfiguration is DiffusionPricerConfiguration diffusionPricerConfiguration) {
                _diffusionConfiguration = new DiffusionConfiguration() {
                    NumberOfDrawings = diffusionPricerConfiguration.NumberOfDrawings,
                    MarketData = marketData,
                    TimeDiscretization = timeDiscretization,
                    Currency = Currencies.USD
                };
            } else {
                _diffusionConfiguration = getDiffusionConfiguration(marketData, timeDiscretization);
            }
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_diffusionConfiguration);
        }

        public PriceWithPrecision Price(
            IContract contract,
            DateTime today,
            Currency pricingCurrency) {

            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }

            IDiscounter discounter = _diffusionConfiguration.MarketData.GetDiscounter(pricingCurrency);

            ValueWithPrecision price = PriceAmerican(contract, today, _diffusion, discounter);
            return new PriceWithPrecision() {
                Value = price.Value,
                Precision = price.Precision,
                Currency = pricingCurrency
            };
        }

        public DiffusionConfiguration getDiffusionConfiguration(IMarketData marketData, List<DateTime> timeDiscretization) {
            IList<Underlying> underlyings = marketData.Underlyings;
            return new DiffusionConfiguration() {
                NumberOfDrawings = 50000,
                MarketData = marketData,
                TimeDiscretization = timeDiscretization,
                Currency = marketData.Currencies.Contains(Currencies.USD) ? Currencies.USD : marketData.Currencies.First()
            };
        }

        public PriceWithPrecision PricePayoff(IPayoff payoff, DateTime today, Currency pricingCurrency) {
            return PricePayoff((IPathDependentPayoff)payoff, today, pricingCurrency);
        }

        private IRegressionBasis _regressionBasis;
        public AmericanPricer(IRegressionBasis? regressionBasis = null) {
            _regressionBasis = regressionBasis ?? new PolynomialRegressionBasis(REGRESSION_DEGREE);
        }

        public ValueWithPrecision PriceAmerican(IContract contract, DateTime valuationDate, Diffusion diffusion, IDiscounter discounter) {

            if (_diffusion == null || _diffusionConfiguration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }

            IEnumerable<DateTime> dates = diffusion.Dates;
            IList<IFlow> flows = ExpandAmericanFlows(contract.Flows, dates);
            IList<DateTime> flowDates = flows.Select(flow => flow.Date).ToList();
            var scenarios = diffusion.Scenarios();

            int N = diffusion.NumberOfEvents;
            int steps = flows.Count();
            Matrix<double> cashFlows = Matrix<double>.Build.Dense(N, steps);
            // Set terminal cash flows at maturity set to payoff
            //IPayoff finalPayoff = (IPayoff) flows.Last();
            
            //cashFlows.SetColumn(steps - 1, scenarios.Select(finalPayoff.ComputePayoff).ToArray());
            for (int step = steps - 1; step >= 0; step--) { // Backward induction
                IFlow flow = flows[step];
                if (flow is IPayoff payoff) {

                    cashFlows.SetColumn(step, scenarios.Select(payoff.ComputePayoff).ToArray());
                
                } else if (flow is IExercisableFlow exercisableFlow) {

                    IPayoff exercisePayoff = exercisableFlow.Payoff;
                    bool[] ITM = scenarios.Select(exercisePayoff.ComputePayoff).Select(payoff => payoff > 0).ToArray();
                    if (!ITM.Any(x => x)) continue;

                    int[] itmIndices            = Enumerable.Range(0, N).Where(j => ITM[j]).ToArray();
                    double[] continuationValues = EstimateContinuationValues(diffusion, cashFlows, step, itmIndices, flowDates, discounter);
                    double[] exerciseValues     = itmIndices.Select(j => exercisePayoff.ComputePayoff(scenarios[j])).ToArray();

                    // Exercise decision
                    for (int k = 0; k < itmIndices.Length; k++) {
                        if (exercisableFlow.ExerciseParty == ExerciseParty.Holder && exerciseValues[k] > continuationValues[k] ||
                            exercisableFlow.ExerciseParty == ExerciseParty.Issuer && exerciseValues[k] < continuationValues[k]) {
                            int j = itmIndices[k];
                            cashFlows.ClearRow(j);
                            cashFlows[j, step] = exerciseValues[k];

                        }
                    }
                }
            }
            // Price = average discounted cash flow across all paths
            IEnumerable<double> pathPrices = Enumerable.Range(0, N)
                .Select(j => GetDiscountedCashFlow(cashFlows, j, 0, flowDates, discounter, valuationDate));
            double price = pathPrices.Average();
            return new ValueWithPrecision(pathPrices);
        }

        private double GetDiscountedCashFlow(Matrix<double> cashFlows, int j, int fromStep, IList<DateTime> callableDates, IDiscounter discounter, DateTime valuationDate) {
            int steps = cashFlows.ColumnCount;
            for (int t = fromStep; t < steps; t++) {
                if (cashFlows[j, t] != 0) {
                    return cashFlows[j, t] * discounter.GetDiscountFactor(callableDates[t], valuationDate);
                }
            }
            return 0.0;
        }

        private double[] EstimateContinuationValues(Diffusion diffusion, Matrix<double> cashFlows, int step, int[] itmIndices, IList<DateTime> callableDates, IDiscounter discounter) {
            // x = (normalized) prices, y = discounted next cash flows
            Dictionary<Underlying, double> spots = diffusion.Spots();
            DateTime stepDate = diffusion.Dates[step];
            var diffusionOnDate = diffusion.ByDate()[stepDate];
            List<Vector<double>> xs = new List<Vector<double>>();
            foreach (Underlying underlying in diffusion.Underlyings) {
                double spot = spots[underlying];
                xs.Add(Vector<double>.Build.DenseOfArray(itmIndices
                    .Select(i => diffusionOnDate[underlying][i] / spot)
                    .ToArray()));
            }
            Vector<double> y = Vector<double>.Build.DenseOfArray(itmIndices
                .Select(j => GetDiscountedCashFlow(cashFlows, j, step + 1, callableDates, discounter, callableDates[step])).ToArray());
            if (itmIndices.Length < REGRESSION_DEGREE) {
                return y.ToArray();
            }
            // Fit regression to estimate continuation value
            Matrix<double> X = _regressionBasis.Build(xs);
            Vector<double> beta = X.Solve(y);
            Vector<double> continuationValues = X.Multiply(beta);
            return continuationValues.ToArray();
        }

        private static IList<IFlow> ExpandAmericanFlows(IEnumerable<IFlow> flows, IEnumerable<DateTime> discretizationDates) {
            List<IFlow> expandedFlows = new();
            foreach (IFlow flow in flows) {
                if (flow is AmericanExercisableFlow americanFlow) {
                    List<DateTime> exercisableDates = discretizationDates.Where(date => date >= americanFlow.StartDate && date <= americanFlow.EndDate).ToList();
                    foreach (DateTime date in exercisableDates) {
                        expandedFlows.Add(new ExercisableFlow() {
                            ExerciseParty = americanFlow.ExerciseParty,
                            Payoff = americanFlow.Payoff,
                            Date = date,
                        });
                    }
                } else {
                    expandedFlows.Add(flow);
                }
            }
            List<IFlow> sorted = expandedFlows.OrderBy(flow => flow.Date).ToList();
            if (sorted.Last() is IExercisableFlow lastFlow) {
                sorted.Remove(lastFlow);
                sorted.Add(lastFlow.Payoff);
            }
            return sorted;

        }
    }
}
