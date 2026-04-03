using Application;
using Domain;
using MarketDataServices;
using MathNet.Numerics.Distributions;
using System;

namespace PricerServices {
    public class EuropeanPutPricer : IPricer<EuropeanPut> {
        private readonly MarketDataService _marketDataService;
        private readonly Random _random;
        public EuropeanPutPricer(MarketDataService marketDataService) {
            _marketDataService = marketDataService;
            _random = new Random();
        }

        public double Price(EuropeanPut contract) {
            // todo
            return 0;
        }       
    }
}
