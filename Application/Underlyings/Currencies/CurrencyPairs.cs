using Domain;

namespace Application {
    public static class CurrencyPairs {
        public static readonly CurrencyPair EURUSD =
            new(Currencies.EUR, Currencies.USD);
        public static readonly CurrencyPair GBPUSD =
            new(Currencies.GBP, Currencies.USD);
        public static readonly CurrencyPair USDJPY =
            new(Currencies.USD, Currencies.JPY);
    }
}
