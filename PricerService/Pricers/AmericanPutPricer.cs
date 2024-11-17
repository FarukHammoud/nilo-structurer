using Application;
using Domain;
using MarketDataServices;
using MathNet.Numerics.Distributions;
using System;

namespace PricerServices {
    public class AmericanPutPricer : IPricer<AmericanPutOption> {
        private readonly MarketDataService _marketDataService;
        private readonly Random _random;
        public AmericanPutPricer(MarketDataService marketDataService) {
            _marketDataService = marketDataService;
            _random = new Random();
        }

        public double Price(AmericanPutOption contract) {
            double spot = _marketDataService.Spot(contract.Underlying);
            double volatility = _marketDataService.Volatility(contract.Underlying);
            const int SAMPLE_SIZE = 100;
            double[] normals = new double[SAMPLE_SIZE];
            Normal.Samples(_random, normals, 0, volatility);
            double sum = 0;
            for (int i = 0; i < SAMPLE_SIZE; i++) {
                sum += contract.Payoff.GetPayoff(spot + normals[i]);
            }
            return sum / SAMPLE_SIZE;
        }       
    }
}
