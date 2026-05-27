using Application;
using Domain;

namespace Infrastructure {
    // To get forward curve from option prices we construct synthetic forwards (C - P)
    // We can try to fit Forward price and discount factor for a specific maturity
    // or from a more trustable discount factor source, only fit the forward price
    public class JsonForwardCurveProvider : IForwardCurveProvider {
        private readonly IOptionPriceProvider _optionPriceProvider;
        private readonly IDiscounter _discounter;
        public JsonForwardCurveProvider(IOptionPriceProvider optionPriceProvider, IDiscounter discounter) {
            _optionPriceProvider = optionPriceProvider;
            _discounter = discounter;
        }

        private static bool IsValidData(OptionMarketData optionMarketData) {
            return optionMarketData.Volume > 5; // bid/ask are 0 out of US market hours
        }

        private static double GetWeight(VanillaContract vanillaContract, OptionMarketData marketData) {
            return marketData.Volume;
        }

        private double GetForwardPrice(double strike, DateTime maturity, double callPrice, double putPrice) {
            double C = callPrice;
            double P = putPrice;
            double K = strike;
            double D = _discounter.GetDiscountFactor(maturity, DateTime.Now);
            return K + (1/D) * (C - P);
        }

        private double VolumeThresholdForward(List<(double Forward, double CallVolume, double PutVolume)> input) {
            double totalVolume = input.Sum(f => Math.Min(f.CallVolume, f.PutVolume));
            List<(double Forward, double minVolume)> orderedForwards = input
                .Select(f => (f.Forward, minVolume: Math.Min(f.CallVolume, f.PutVolume)))
                .OrderByDescending(f => f.minVolume)
                .ToList();
            double volumeThreshold = 0.75 * totalVolume;
            double volume = 0;
            double forward = 0;
            while (volume < volumeThreshold) {
                (double f, double v) = orderedForwards.First();
                forward += f * v;
                volume += v;
                orderedForwards.RemoveAt(0);
            }
            return forward/volume;
        }

        public async Task<Dictionary<Underlying, Curve>> GetForwardCurveAsync(IEnumerable<Underlying> underlyings, CancellationToken ct = default) {
            Dictionary<Underlying, Dictionary<VanillaContract, OptionMarketData>> optionData = await _optionPriceProvider.GetOptionPricesAsync(underlyings, ct);
            Dictionary<Underlying, Curve> forwardCurves = new();
            foreach (Underlying underlying in optionData.Keys) {
                Dictionary<DateTime, Dictionary<double, Dictionary<VanillaContract, OptionMarketData>>> optionsMarketData = optionData[underlying]
                    .Where(pair => IsValidData(pair.Value))
                    .GroupBy(pair => pair.Key.Maturity)
                    .ToDictionary(
                        maturityGroup => maturityGroup.Key,
                        maturityGroup => maturityGroup
                            .GroupBy(pair => pair.Key.Strike)
                            .ToDictionary(
                                strikeGroup => strikeGroup.Key, 
                                strikeGroup => strikeGroup.ToDictionary(
                                    pair => pair.Key,  
                                    pair => pair.Value  
                                )
                            )
                    );
                Curve forwardCurve = new();
                foreach (DateTime maturity in optionsMarketData.Keys) {
                    Dictionary<double, Dictionary<VanillaContract, OptionMarketData>> optionsByMaturity = optionsMarketData[maturity];
                    List<(double Forward, double CallVolume, double PutVolume)> forwards = new();
                    foreach (double strike in optionsByMaturity.Keys) {
                        Dictionary<VanillaContract, OptionMarketData> marketDataByContract = optionsByMaturity[strike];
                        if (marketDataByContract.Count(a => a.Key is ICall) != 1 || marketDataByContract.Count(a => a.Key is IPut) != 1) {
                            continue;
                        }
                        VanillaContract call = marketDataByContract.Keys.First(a => a is ICall) as VanillaContract;
                        OptionMarketData callMarketData = marketDataByContract[call];
                        VanillaContract put = marketDataByContract.Keys.First(a => a is IPut) as VanillaContract;
                        OptionMarketData putMarketData = marketDataByContract[put];
                        double forwardPrice = GetForwardPrice(strike, maturity, callMarketData.LastPrice, putMarketData.LastPrice);
                        forwards.Add((forwardPrice, callMarketData.Volume, putMarketData.Volume));
                    }
                    if (!forwards.Any()) {
                        continue;
                    }
                    double weightedForward = VolumeThresholdForward(forwards);
                    forwardCurve.setNode(maturity, weightedForward);
                }
                forwardCurves.Add(underlying, forwardCurve);
            }
            return forwardCurves;
        }
    }
}
