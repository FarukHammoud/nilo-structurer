using Domain;

namespace Application {
    /// <summary>
    /// PricingEngine dispatches pricing requests to the appropriate pricers based on the contract type and model configuration. It handles both path-independent and path-dependent contracts, constructs the necessary time grids, and aggregates results for various indicators.
    /// </summary>
    public class PricingEngine : IPricingEngine {

        private readonly IPricerFactory _pricerFactory;
        private readonly ITimeGridBuilder _timeGridBuilder;

        public PricingEngine(IPricerFactory? pricerFactory = null, ITimeGridBuilder? timeGridBuilder = null) {
            _pricerFactory = pricerFactory ?? new PricerFactory();
            _timeGridBuilder = timeGridBuilder ?? new TimeGridBuilder();
        }

        // Target signature for the asynchronous pricing method
        public async Task<Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>>> RunAsync(PricingRequest request,
            IProgress<PricingProgress>? progress = null,
            CancellationToken cancellationToken = default) {
            return await Task.Run(() => {
                return Run(request);
            }, cancellationToken);
        }

        public Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> Run(PricingRequest request) {
            Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults = new();

            HashSet<(IMarketData, DateTime)> shiftedMarketData = request.Indicators
                .SelectMany(indicator => indicator.GetShiftedMarketData(request.MarketData, request.PricingDate))
                .ToHashSet();

            IList<DateTime> timeGrid                 = _timeGridBuilder.Build(request.Position, request.ModelConfiguration, request.PricingDate);
            IPricerConfiguration pricerConfiguration = _pricerFactory.CreateConfiguration(request);

            foreach ((IMarketData marketData, DateTime pricingDate) in shiftedMarketData) {
                subResults[(marketData, pricingDate)] = new();
                foreach (IContract contract in request.Position) {
                    IPricer pricer         = _pricerFactory.CreatePricer(request.ModelConfiguration);
                    pricer.Initialize(marketData, timeGrid, pricerConfiguration);
                    subResults[(marketData, pricingDate)][contract] = pricer.Price(contract, pricingDate, request.PricingCurrency);
                }
            }
            if (request.ModelConfiguration.Discounting is StochasticRatesDiscounting) {

            }

            return GetIndicatorResults(request, subResults);
        }

        private Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> GetIndicatorResults(PricingRequest request, Dictionary<(IMarketData, DateTime), Dictionary<IContract, PriceWithPrecision>> subResults) {
            // Transform subResults to the desired output format
            Dictionary<IContract, Dictionary<(IMarketData, DateTime), PriceWithPrecision>> pivotedSubResults = subResults.Pivot();
            Dictionary<IContract, Dictionary<IIndicator, IIndicatorResult>> indicatorResult = new();
            foreach (IContract contract in request.Position) {
                indicatorResult.Add(contract, new());
                foreach (IIndicator indicator in request.Indicators) {
                    IIndicatorResult result = indicator.GetResult(
                        contract: contract,
                        unshiftedMarketData: request.MarketData,
                        pricingDate: request.PricingDate,
                        resultsByShift: pivotedSubResults[contract]);
                    indicatorResult[contract].Add(indicator, result);
                }
            }
            return indicatorResult;
        }
    }
}
