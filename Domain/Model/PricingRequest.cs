namespace Domain {
    public class PricingRequest {
        public required ModelConfiguration ModelConfiguration { get; set; }
        public required List<Contract> Position { get; set; }
        public required IMarketData MarketData { get; set; }
        public required List<IIndicator> Indicators { get; set; }
        public DateTime PricingDate { get; set; }
    }
}
