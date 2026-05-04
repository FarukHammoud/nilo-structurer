namespace Domain {
    public record CurrencyPair : Underlying {
        public Currency Base { get; }   // EUR in EURUSD
        public Currency Quote { get; }  // USD in EURUSD
        public override Currency Currency => Quote;

        public CurrencyPair(Currency @base, Currency quote)
            : base($"{@base.Code}{quote.Code}") {
            Base = @base;
            Quote = quote;
        }

        public override IEnumerable<Underlying> Dependencies => [this];
    }
}
