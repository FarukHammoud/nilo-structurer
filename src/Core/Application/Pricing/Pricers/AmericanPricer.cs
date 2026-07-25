using Domain;
using MathNet.Numerics.LinearAlgebra;

namespace Application {
    public class AmericanPricer : IPricer {

        private IDiffusionConfiguration? _configuration;
        private Diffusion? _diffusion;
        private const int REGRESSION_DEGREE = 3;

        public void Initialize(IMarketData marketData, IList<DateTime> timeDiscretization, IPricerConfiguration? pricerConfiguration = null) {
            if (pricerConfiguration is DiffusionPricerConfiguration diffusionConfiguration) {
                _configuration = new DiffusionConfiguration() {
                    NumberOfDrawings = diffusionConfiguration.NumberOfDrawings,
                    MarketData = marketData,
                    TimeDiscretization = timeDiscretization,
                    Currency = Currencies.USD,
                    HasStochasticRate = diffusionConfiguration.HasStochasticRate,
                };
            } else {
                _configuration = getDiffusionConfiguration(marketData, timeDiscretization);
            }
            _diffusion = GeneralDiffusion.DiffuseMultiUnderlying(_configuration);
        }

        public PriceEstimate Price(
            IContract contract,
            DateTime today,
            Currency pricingCurrency) {

            if (_diffusion == null || _configuration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }

            IDiscounter discounter = _configuration.MarketData.GetDiscounter(pricingCurrency);

            Estimate price = PriceAmerican(contract, today, _diffusion, discounter);
            return new PriceEstimate(price, pricingCurrency);
        }

        public IDiffusionConfiguration getDiffusionConfiguration(IMarketData marketData, IList<DateTime> timeDiscretization) {
            IList<Underlying> underlyings = marketData.Underlyings;
            return new DiffusionConfiguration() {
                NumberOfDrawings = 50000,
                MarketData = marketData,
                TimeDiscretization = timeDiscretization,
                Currency = marketData.Currencies.Contains(Currencies.USD) ? Currencies.USD : marketData.Currencies.First()
            };
        }

        private IRegressionBasis _regressionBasis;
        public AmericanPricer(IRegressionBasis? regressionBasis = null) {
            _regressionBasis = regressionBasis ?? new PolynomialRegressionBasis(REGRESSION_DEGREE);
        }

        public Estimate PriceAmerican(IContract contract, DateTime valuationDate, Diffusion diffusion, IDiscounter discounter) {

            if (_diffusion == null || _configuration == null) {
                throw new Exception("Pricer not initialized. Please call Initialize method before pricing.");
            }

            IEnumerable<DateTime> dates = diffusion.Dates;
            IList<IFlow> flows = ExpandAmericanFlows(contract.Flows, dates);
            IList<DateTime> flowDates = flows.Select(flow => flow.Date).ToList();
            List<Scenario> scenarios = diffusion.Scenarios();

            int N = diffusion.NumberOfEvents;
            int steps = flows.Count();
            Matrix<double> cashFlows = Matrix<double>.Build.Dense(N, steps);
            // Backward 
            for (int step = steps - 1; step >= 0; step--) {
                IFlow flow = flows[step];
                if (flow is IPayoff payoff) {

                    cashFlows.SetColumn(step, scenarios.Select(payoff.ComputePayoff).ToArray());
                } else if (flow is IAutoCallFlow autoCallFlow) {

                    for (int j = 0; j < N; j++) {
                        if (autoCallFlow.IsTriggered(scenarios[j])) {
                            cashFlows.ClearRow(j);
                            cashFlows[j, step] = autoCallFlow.Rebate.ComputePayoff(scenarios[j]);
                        }
                    }
                } else if (flow is IExercisableFlow exercisableFlow) {

                    IPayoff exercisePayoff = exercisableFlow.Payoff;

                    double[] continuationValues = EstimateContinuationValues(diffusion, cashFlows, step, flowDates, discounter);
                    double[] exerciseValues     = scenarios.Select(exercisePayoff.ComputePayoff).ToArray();

                    // Exercise decision
                    for (int j = 0; j < N; j++) {
                        if (exercisableFlow.ExerciseParty == ExerciseParty.Holder && exerciseValues[j] > continuationValues[j] ||
                            exercisableFlow.ExerciseParty == ExerciseParty.Issuer && exerciseValues[j] < continuationValues[j]) {
                            cashFlows.ClearRow(j);
                            cashFlows[j, step] = exerciseValues[j];
                        }
                    }
                }
            }
            // Price = average discounted cash flow across all paths
            IEnumerable<double> pathPrices = Enumerable.Range(0, N)
                .Select(j => GetDiscountedCashFlow(cashFlows, j, 0, flowDates, discounter, valuationDate));
            double price = pathPrices.Average();
            return new Estimate(pathPrices);
        }

        private double GetDiscountedCashFlow(Matrix<double> cashFlows, int j, int fromStep, IList<DateTime> flowDates, IDiscounter discounter, DateTime valuationDate) {
            int steps = cashFlows.ColumnCount;
            int paths = cashFlows.RowCount;
            double sum = 0;
            if (_configuration.HasStochasticRate) {
                IDictionary<Currency, ShortRate> shortRates = _configuration.Underlyings.OfType<ShortRate>().ToDictionary(x => x.Currency, x => x);
                if (shortRates.ContainsKey(_configuration.Currency)) {
                    for (int t = fromStep; t < steps; t++) {
                        ShortRate shortRate         = shortRates[_configuration.Currency];
                        IList<DateTime> dates       = _configuration.TimeDiscretization;
                        SimulatedPath shortRatePath = _diffusion[shortRate][j];
                        ShortRateDiscounter stochasticDiscounter = new ShortRateDiscounter(shortRatePath, dates);
                        double stochasticDF = stochasticDiscounter.GetDiscountFactor(flowDates[t], valuationDate);
                        sum += cashFlows[j, t] * stochasticDF;
                    }
                }
            } else {
                for (int t = fromStep; t < steps; t++) {
                    sum += cashFlows[j, t] * discounter.GetDiscountFactor(flowDates[t], valuationDate);
                }
            }
            return sum;
        }

        private double[] EstimateContinuationValues(Diffusion diffusion, Matrix<double> cashFlows, int step, IList<DateTime> flowDates, IDiscounter discounter) {
            // x = (normalized) prices, y = discounted next cash flows
            int N = diffusion.NumberOfEvents;
            Dictionary<Underlying, double> spots = diffusion.Spots();
            DateTime stepDate = flowDates[step];
            var diffusionOnDate = diffusion[stepDate];
            List<Vector<double>> xs = new List<Vector<double>>();
            foreach (Underlying underlying in diffusion.Underlyings) {
                double spot = spots[underlying];
                xs.Add(Vector<double>.Build.DenseOfArray(diffusionOnDate[underlying]
                    .Select(value => value / spot)
                    .ToArray()));
            }
            Vector<double> y = Vector<double>.Build.DenseOfArray(Enumerable.Range(0, N)
                .Select(j => GetDiscountedCashFlow(cashFlows, j, step + 1, flowDates, discounter, flowDates[step])).ToArray());
            if (N < REGRESSION_DEGREE) {
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
