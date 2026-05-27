namespace Domain {
    public record Equity : Underlying {
        public String Name { get; set; }
        public override Currency Currency { get; }
        public Equity(string code, Currency currency) : base(code) {
            Name = code;
            Currency = currency;
        }

        public override IEnumerable<Underlying> Dependencies => [this];
    }
}
