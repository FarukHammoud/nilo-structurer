using Application;
using Domain;
using MarketDataServices;
using MathNet.Numerics.Distributions;
using System;

namespace PricerServices {
    public class EuropeanCallPricer : IPricer<EuropeanCall> {
        private readonly MarketDataService _marketDataService;
        private readonly Random _random;
        public EuropeanCallPricer(MarketDataService marketDataService) {
            _marketDataService = marketDataService;
            _random = new Random();
        }

        public double Price(EuropeanCall contract) {
            // todo
            return 0;
        }       
    }
}
