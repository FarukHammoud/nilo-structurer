namespace Application {
    public record OptionMarketData {
         public double LastPrice { get; init; }
         public double Volume { get; init; }
         public double OpenInterest { get; init; }
         public double Bid { get; init; }
         public double Ask { get; init; }

    }
}
