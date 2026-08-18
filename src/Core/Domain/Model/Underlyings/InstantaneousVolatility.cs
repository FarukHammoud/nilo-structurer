namespace Domain {
    public record InstantaneousVolatility : Underlying {
        public Equity Equity { get; init; }
        public override Currency Currency => Equity.Currency;
        public InstantaneousVolatility(Equity equity) : base(equity.Code + "_VOLATILITY") {
            Equity = equity;
        }
        public override IEnumerable<Underlying> Dependencies => [this];
    }
}
